using System;
using System.Collections.Generic;

namespace Unity.AppUI.Undo
{
    /// <summary>
    /// <para>
    /// An undo stack maintains a stack of commands that have been applied in your application.
    /// </para>
    /// <para>
    /// New commands are pushed on the stack using <see cref="Push"/>.
    /// Commands can be undone and redone using <see cref="Undo"/> and <see cref="Redo"/>.
    /// </para>
    /// <para>
    /// UndoStack keeps track of the current command.
    /// This is the command which will be executed by the next call to <see cref="Redo"/>.
    /// The index of this command is returned by <see cref="index"/>.
    /// The state of the edited object can be rolled forward or back using <see cref="index"/>.
    /// If the top-most command on the stack has already been redone, <see cref="index"/> is equal to <see cref="count"/>.
    /// </para>
    /// <para>
    /// UndoStack provides *command compression*,
    /// command *macros*, and supports the concept of a *clean state*.
    /// </para>
    /// </summary>
    public class UndoStack
    {
        readonly List<UndoCommand> m_Commands = new List<UndoCommand>();

        readonly Stack<MacroCommand> m_MacroStack = new Stack<MacroCommand>();

        int m_CleanState = -1;

        int m_Index = -1;

        /// <summary>
        /// Emitted the current Command index changes.
        /// </summary>
        public event Action<int> indexChanged;

        /// <summary>
        /// Emitted when the clean state changes.
        /// </summary>
        public event Action cleanStateChanged;

        /// <summary>
        /// Weather the redo stack is empty.
        /// </summary>
        public bool canRedo => m_Index < m_Commands.Count - 1 && m_MacroStack.Count == 0;

        /// <summary>
        /// Weather the undo stack is empty.
        /// </summary>
        public bool canUndo => m_Index >= 0 && m_MacroStack.Count == 0;

        /// <summary>
        /// The number of commands in the stack.
        /// Macros are counted as a single command.
        /// </summary>
        public int count => m_Commands.Count;

        /// <summary>
        /// Weather the undo stack is in a clean state.
        /// The clean state is useful when the application supports saving and restoring the state of an object.
        /// </summary>
        public bool isClean => (m_Commands.Count == 0 && m_CleanState == -1) || m_CleanState == m_Index;

        /// <summary>
        /// The index of the clean state. -1 if there is no clean state.
        /// </summary>
        public int cleanIndex => m_CleanState;

        /// <summary>
        /// The index of the last command in the undo stack.
        /// </summary>
        public int index
        {
            get => m_Index;
            set => SetIndexInternal(value);
        }

        /// <summary>
        /// The maximum number of memory units that can be stored in the undo stack.
        /// </summary>
        public ulong undoLimit { get; set; } = 1000000;

        /// <summary>
        /// The total memory size of the undo stack.
        /// </summary>
        public ulong memorySize
        {
            get
            {
                ulong size = 0;
                foreach (var command in m_Commands)
                {
                    size += command.memorySize;
                }
                return size;
            }
        }

        /// <summary>
        /// The last command in the undo stack.
        /// </summary>
        public UndoCommand lastCommand => m_Index >= 0 ? m_Commands[m_Index] : null;

        /// <summary>
        /// The whole commands list.
        /// </summary>
        public IEnumerable<UndoCommand> commands => m_Commands;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public UndoStack()
        {

        }

        void SetIndexInternal(int value)
        {
            if (m_MacroStack.Count > 0)
                throw new InvalidOperationException("Cannot set index while in a macro composition.");

            if (index == value)
                return;

            if (value < 0 || value > count)
                throw new ArgumentOutOfRangeException(nameof(value), value, "Index out of range.");

            var prev = index;
            if (value < index)
            {
                while (index > value)
                    UndoWithoutNotify();
            }
            else
            {
                while (index < value)
                    RedoWithoutNotify();
            }

            if (prev != index)
                indexChanged?.Invoke(index);
        }

        /// <summary>
        /// Pushes the given command on the undo stack.
        /// </summary>
        /// <remarks>
        /// If the current command is a macro command, the pushed command is added to the macro.
        /// </remarks>
        /// <param name="command"> The command to push. </param>
        public void Push(UndoCommand command)
        {
            if (command.memorySize > undoLimit)
                throw new InvalidOperationException("The pushed command is too big to fit in the undo stack.");

            if (m_MacroStack.TryPeek(out var macro))
            {
                macro.Add(command);
                return;
            }

            if (m_Index < m_Commands.Count - 1)
            {
                for (var i = m_Commands.Count - 1; i > m_Index; i--)
                {
                    m_Commands[i].OnFlush();
                    m_Commands.RemoveAt(i);
                }
                if (m_CleanState > m_Index)
                    m_CleanState = -1;
            }

            while (memorySize + command.memorySize > undoLimit)
            {
                if (m_Commands.Count == 0)
                    break;

                m_Commands[0].OnFlush();
                m_Commands.RemoveAt(0);
                m_Index--;
                m_CleanState = Math.Max(m_CleanState - 1, -1);
            }

            m_Commands.Add(command);
            m_Index++;

            indexChanged?.Invoke(index);
        }

        /// <summary>
        /// Calls <see cref="UndoCommand.Undo"/> on the last command in the undo stack.
        /// </summary>
        public void Undo()
        {
            var prev = m_Index;
            UndoWithoutNotify();
            if (prev != m_Index)
                indexChanged?.Invoke(index);
        }

        void UndoWithoutNotify()
        {
            if (m_Commands.Count == 0 || m_MacroStack.Count > 0 || m_Index < 0)
                return;

            var command = m_Commands[m_Index];
            command.Undo();
            m_Index--;
        }

        /// <summary>
        /// Calls <see cref="UndoCommand.Redo"/> on the last command in the redo stack.
        /// </summary>
        public void Redo()
        {
            var prev = m_Index;
            RedoWithoutNotify();
            if (prev != m_Index)
                indexChanged?.Invoke(index);
        }

        void RedoWithoutNotify()
        {
            if (m_Commands.Count == 0 || m_MacroStack.Count > 0 || m_Index >= m_Commands.Count - 1)
                return;

            var command = m_Commands[m_Index + 1];
            command.Redo();
            m_Index++;
        }

        /// <summary>
        /// Clears the undo and redo stacks.
        /// </summary>
        public void Clear()
        {
            m_MacroStack.Clear();

            var wasClean = isClean;
            if (m_Commands.Count != 0)
            {
                for (var i = m_Commands.Count - 1; i >= 0; i--)
                {
                    m_Commands[i].OnFlush();
                }
                m_Commands.Clear();
                m_Index = -1;
                indexChanged?.Invoke(index);
            }

            m_CleanState = -1;

            if (!wasClean)
                cleanStateChanged?.Invoke();
        }

        /// <summary>
        /// Sets the clean state to the last command in the undo stack.
        /// </summary>
        public void SetClean()
        {
            if (isClean)
                return;

            m_CleanState = m_Index;

            cleanStateChanged?.Invoke();
        }

        /// <summary>
        /// Sets the clean state to -1.
        /// </summary>
        public void ResetClean()
        {
            if (m_CleanState == -1)
                return;

            m_CleanState = -1;

            cleanStateChanged?.Invoke();
        }

        /// <summary>
        /// Gets the command at the given index.
        /// </summary>
        /// <param name="idx"> The index of the command. </param>
        /// <value> The command at the given index, or null if the index is out of range. </value>
        public UndoCommand this[int idx] => idx >= 0 && idx < m_Commands.Count ? m_Commands[idx] : null;

        /// <summary>
        /// <para>
        /// Begins composition of a macro command with the given text description.
        /// </para>
        /// <para>
        /// An empty command described by the specified text is pushed on the stack.
        /// Any subsequent commands pushed on the stack will be appended to the macro command's children until
        /// <see cref="EndMacro"/> is called.
        /// </para>
        /// <para>
        /// While a macro is being composed, the stack is disabled. This means that:<br/>
        /// - Events are not emitted.<br/>
        /// - <see cref="canRedo"/> and <see cref="canUndo"/> will always return false.<br/>
        /// - Calling <see cref="Undo"/> or <see cref="Redo"/> has no effect.<br/>
        /// The stack is re-enabled when the macro is ended.
        /// </para>
        /// <para>
        /// Here is an example of how to use macros:
        /// <c>
        /// undoStack.BeginMacro("Insert Red Text");
        /// undoStack.Push(new InsertText(...));
        /// undoStack.Push(new SetRedText(...));
        /// undoStack.EndMacro();
        /// </c>
        /// This code is equivalent to:
        /// <c>
        /// var insertRedText = new UndoCommand("Insert Text");
        /// new InsertText(..., insertRedText);
        /// new SetRedText(..., insertRedText);
        /// undoStack.Push(insertRedText);
        /// </c>
        /// </para>
        /// </summary>
        /// <remarks>
        /// Nested calls to <see cref="BeginMacro"/> are supported, but every call to <see cref="BeginMacro"/> must have
        /// a corresponding call to <see cref="EndMacro"/>.
        /// </remarks>
        /// <seealso cref="EndMacro"/>
        /// <param name="name"> The name of the macro command. </param>
        public void BeginMacro(string name)
        {
            var macro = new MacroCommand(name);
            m_MacroStack.Push(macro);
        }

        /// <summary>
        /// Ends composition of a macro command.<br/> If this is the outermost macro in a set nested macros,
        /// this function emits <see cref="indexChanged"/> once for the entire macro command.
        /// </summary>
        /// <seealso cref="BeginMacro"/>
        /// <exception cref="InvalidOperationException"> Thrown if no macro is being composed. </exception>
        public void EndMacro()
        {
            if (m_MacroStack.TryPop(out var macro))
            {
                Push(macro);
            }
            else
            {
                throw new InvalidOperationException("Cannot end macro: no macro is being composed.");
            }
        }
    }
}

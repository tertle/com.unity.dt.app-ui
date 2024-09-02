using System.Collections.Generic;

namespace Unity.AppUI.Undo
{
    /// <summary>
    /// Base class for undoable commands.
    /// </summary>
    public abstract class UndoCommand
    {
        /// <summary>
        /// The unique identifier of the command type.
        /// </summary>
        public abstract string id { get; }

        /// <summary>
        /// The memory size of the command.
        /// </summary>
        public abstract ulong memorySize { get; }

        /// <summary>
        /// The name of the command.
        /// </summary>
        public string name { get; }

        /// <summary>
        /// Whether the command is obsolete.
        /// </summary>
        public bool isObsolete { get; set; }

        /// <summary>
        /// Creates a new undo command.
        /// </summary>
        /// <param name="name"> The name of the command. </param>
        protected UndoCommand(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Undoes the command.
        /// </summary>
        public abstract void Undo();

        /// <summary>
        /// Redoes the command.
        /// </summary>
        public abstract void Redo();

        /// <summary>
        /// Called when the command is flushed.
        /// </summary>
        public abstract void OnFlush();

        /// <summary>
        /// Merges the command with another command.
        /// </summary>
        /// <param name="command"> The command to merge with. </param>
        /// <returns> Whether the command was merged. </returns>
        public virtual bool MergeWith(UndoCommand command)
        {
            return false;
        }
    }

    /// <summary>
    /// A macro command is a command that contains other commands.
    /// </summary>
    public class MacroCommand : UndoCommand
    {
        readonly List<UndoCommand> m_Commands = new List<UndoCommand>();

        /// <summary>
        /// Creates a new macro command.
        /// </summary>
        /// <param name="name"> The name of the macro command. </param>
        public MacroCommand(string name)
            : base(name) { }

        /// <summary>
        /// The unique identifier of the macro command type.
        /// </summary>
        public override string id => nameof(MacroCommand);

        /// <summary>
        /// The memory size of the macro command.
        /// </summary>
        public override ulong memorySize
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
        /// Adds a command to the macro command.
        /// </summary>
        /// <param name="command"> The command to add. </param>
        public void Add(UndoCommand command)
        {
            m_Commands.Add(command);
        }

        /// <summary>
        /// Undoes the macro command.
        /// </summary>
        public override void Undo()
        {
            for (var i = m_Commands.Count - 1; i >= 0; i--)
            {
                m_Commands[i].Undo();
            }
        }

        /// <summary>
        /// Redoes the macro command.
        /// </summary>
        public override void Redo()
        {
            foreach (var command in m_Commands)
            {
                command.Redo();
            }
        }

        /// <summary>
        /// This will call <see cref="UndoCommand.OnFlush"/> on all the commands contained in the macro command.
        /// </summary>
        public override void OnFlush()
        {
            for (var i = m_Commands.Count - 1; i >= 0; i--)
            {
                m_Commands[i].OnFlush();
            }

            m_Commands.Clear();
        }

        /// <summary>
        /// Get the command at the specified index.
        /// </summary>
        /// <param name="index"> The index of the command. </param>
        public UndoCommand this[int index] => m_Commands.Count > index && index >= 0 ? m_Commands[index] : null;
    }
}

package com.unity3d.player.appui;

import android.content.Context;
import android.content.res.Configuration;
import android.content.res.Resources;
import android.database.ContentObserver;
import android.os.Build;
import android.os.Bundle;
import android.os.VibrationEffect;
import android.os.Vibrator;
import android.os.Handler;
import android.os.VibratorManager;
import android.provider.Settings;
import android.util.DisplayMetrics;
import android.util.Log;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import java.util.Locale;
import android.text.TextUtils;
import android.view.accessibility.AccessibilityManager;
// import android.view.accessibility.AccessibilityManager.HighTextContrastChangeListener;

import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerGameActivity;

/** @noinspection unused*/
public class AppUIGameActivity extends UnityPlayerGameActivity /*implements HighTextContrastChangeListener*/ {

    int m_CurrentUiMode = Configuration.UI_MODE_NIGHT_UNDEFINED;
    float m_CurrentFontScale = 1;
    float m_CurrentScaledDensity = 1;
    float m_CurrentDensity = 1;
    int m_CurrentDensityDpi = DisplayMetrics.DENSITY_DEFAULT;
    boolean m_CurrentHighContrast = false;
    boolean m_CurrentReduceMotion = false;
    int m_CurrentLayoutDirection = 0;
    Vibrator m_Vibrator;

    public boolean IsNightModeDefined() {
        return m_CurrentUiMode != Configuration.UI_MODE_NIGHT_UNDEFINED;
    }

    public boolean IsNightModeEnabled() {
        return m_CurrentUiMode == Configuration.UI_MODE_NIGHT_YES;
    }

    public float FontScale() {
        return m_CurrentFontScale;
    }

    public float ScaledDensity() {
        return m_CurrentScaledDensity;
    }

    public float Density() {
        return m_CurrentDensity;
    }

    public int DensityDpi() {
        return m_CurrentDensityDpi;
    }

    public boolean HighContrast() {
        return m_CurrentHighContrast;
    }

    public boolean ReduceMotion() {
        return m_CurrentReduceMotion;
    }

    public int LayoutDirection() {
        return m_CurrentLayoutDirection;
    }

    void Vibrate(long[] timings, int repeat) {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            m_Vibrator.vibrate(VibrationEffect.createWaveform(timings, repeat));
        } else {
            m_Vibrator.vibrate(timings, repeat);
        }
    }

    void Vibrate(long[] timings) {
        Vibrate(timings, -1);
    }

    void Vibrate(long timing, int amplitude) {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            m_Vibrator.vibrate(VibrationEffect.createOneShot(timing, amplitude));
        }
        else {
            m_Vibrator.vibrate(timing);
        }
    }

    void Vibrate(long timing) {
        Vibrate(timing, -1);
    }

    static boolean IsHighContrastEnabled(Context context) {
      AccessibilityManager accessibilityManager =
          (AccessibilityManager) context.getSystemService(Context.ACCESSIBILITY_SERVICE);
      Method highContrastMethod = null;
      if (accessibilityManager != null) {
        try {
          highContrastMethod = accessibilityManager.getClass().getMethod("isHighTextContrastEnabled", null);
        }
        catch(NoSuchMethodException ignored) {}
      }

      Object highContrast = null;
      if (highContrastMethod != null) {
        try {
          highContrast = highContrastMethod.invoke(accessibilityManager, null);
        }
        catch(IllegalAccessException | InvocationTargetException ignored) {}
      }

      if (highContrast instanceof Boolean) {
        return (boolean) highContrast;
      }

      return false;
    }

    /*
    void AddHighContrastChangeListener() {
      AccessibilityManager accessibilityManager = (AccessibilityManager) getSystemService(Context.ACCESSIBILITY_SERVICE);
      Method addHighTextContrastStateChangeListenerMethod = null;
      if (accessibilityManager != null) {
        try {
          if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            addHighTextContrastStateChangeListenerMethod =
                          accessibilityManager.getClass().getMethod(
                                  "addHighTextContrastStateChangeListener",
                                  HighTextContrastChangeListener.class,
                                  Handler.class);
          }
          else {
            addHighTextContrastStateChangeListenerMethod =
                          accessibilityManager.getClass().getMethod(
                                  "addHighTextContrastStateChangeListener",
                                  HighTextContrastChangeListener.class);
          }
        }
        catch(NoSuchMethodException ignored) {}
      }

      if (addHighTextContrastStateChangeListenerMethod != null) {
        try {
          if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            addHighTextContrastStateChangeListenerMethod.invoke(accessibilityManager, getApplicationContext(), null);
          }
          else {
            addHighTextContrastStateChangeListenerMethod.invoke(accessibilityManager, getApplicationContext());
          }
        }
        catch(IllegalAccessException | InvocationTargetException ignored) {}
      }
    }
    */

    static boolean IsRemoveAnimationEnabled(Context context) {
      return Settings.Global.getFloat(context.getContentResolver(), Settings.Global.WINDOW_ANIMATION_SCALE, 1.0f) == 0.0f
        && Settings.Global.getFloat(context.getContentResolver(), Settings.Global.TRANSITION_ANIMATION_SCALE, 1.0f) == 0.0f
        && Settings.Global.getFloat(context.getContentResolver(), Settings.Global.ANIMATOR_DURATION_SCALE, 1.0f) == 0.0f;
    }

    void AddReduceMotionChangeListener()
    {
      ContentObserver observer = new ContentObserver(new Handler()) {
         @Override
         public void onChange(boolean selfChange) {
            super.onChange(selfChange);
            boolean reduceMotion = IsRemoveAnimationEnabled(getApplicationContext());
            if (m_CurrentReduceMotion != reduceMotion) {
              m_CurrentReduceMotion = reduceMotion;
              UnityPlayer.UnitySendMessage("AppUIUpdater", "OnNativeMessageReceived", "configurationChanged");
            }
         }
         @Override
         public boolean deliverSelfNotifications() {
            return true;
         }
      };
      getContentResolver()
        .registerContentObserver(Settings.Global.getUriFor(
                Settings.Global.WINDOW_ANIMATION_SCALE),
                false,
                observer);
    }

    static int GetLayoutDirection(Configuration configuration) {
      Locale currentLocale = null;
      if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N) {
        currentLocale = configuration.getLocales().get(0);
      } else {
        currentLocale = configuration.locale;
      }
      return TextUtils.getLayoutDirectionFromLocale(currentLocale);
    }

    public void RunHapticFeedback(int hapticFeedbackType) {

        switch (hapticFeedbackType) {
            case HapticFeedback.LIGHT: {
                Vibrate(HapticFeedback.lightTiming, HapticFeedback.lightAmplitude);
                break;
            }
            case HapticFeedback.MEDIUM: {
                Vibrate(HapticFeedback.mediumTiming, HapticFeedback.mediumAmplitude);
                break;
            }
            case HapticFeedback.HEAVY: {
                Vibrate(HapticFeedback.heavyTiming, HapticFeedback.heavyAmplitude);
                break;
            }
            case HapticFeedback.SUCCESS: {
                Vibrate(HapticFeedback.successTimings);
                break;
            }
            case HapticFeedback.ERROR: {
                Vibrate(HapticFeedback.errorTimings);
                break;
            }
            case HapticFeedback.WARNING: {
                Vibrate(HapticFeedback.warningTimings);
                break;
            }
            case HapticFeedback.SELECTION: {
                Vibrate(HapticFeedback.selectionTiming, HapticFeedback.lightAmplitude);
                break;
            }
            default:
                break;
        }
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        Resources resources = getResources();

        Configuration configuration = resources.getConfiguration();

        m_CurrentUiMode = configuration.uiMode & Configuration.UI_MODE_NIGHT_MASK;
        m_CurrentFontScale = configuration.fontScale;

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.S) {
            VibratorManager manager = (VibratorManager) getSystemService(Context.VIBRATOR_MANAGER_SERVICE);
            m_Vibrator = manager.getDefaultVibrator();
        }
        else {
            m_Vibrator = (Vibrator) getSystemService(Context.VIBRATOR_SERVICE);
        }

        DisplayMetrics displayMetrics = resources.getDisplayMetrics();
        m_CurrentScaledDensity = displayMetrics.scaledDensity;
        m_CurrentDensity = displayMetrics.density;
        m_CurrentDensityDpi = displayMetrics.densityDpi;

        m_CurrentHighContrast = IsHighContrastEnabled(this);
        // AddHighContrastChangeListener();
        m_CurrentReduceMotion = IsRemoveAnimationEnabled(this);
        AddReduceMotionChangeListener();
        m_CurrentLayoutDirection = GetLayoutDirection(configuration);

        Log.d("APP UI", "Initial Night Mode: " + m_CurrentUiMode);
        Log.d("APP UI", "Initial Font Scale: " + m_CurrentFontScale);
        Log.d("APP UI", "Initial Density: " + displayMetrics.density);
        Log.d("APP UI", "Initial Scaled Density: " + displayMetrics.scaledDensity);
        Log.d("APP UI", "Initial Density DPI: " + displayMetrics.densityDpi);
        Log.d("APP UI", "Initial High Contrast: " + m_CurrentHighContrast);
        Log.d("APP UI", "Initial Reduce Motion: " + m_CurrentReduceMotion);
        Log.d("APP UI", "Initial Layout Direction: " + m_CurrentLayoutDirection);
    }

    /*
    @Override
    public void onHighTextContrastStateChanged(boolean enabled) {
        m_CurrentHighContrast = enabled;
        UnityPlayer.UnitySendMessage("AppUIUpdater", "OnNativeMessageReceived", "configurationChanged");
    }
    */

    @Override
    public void onConfigurationChanged(Configuration newConfig) {
        super.onConfigurationChanged(newConfig);

        DisplayMetrics displayMetrics = getResources().getDisplayMetrics();
        boolean configurationChanged = false;

        int currentNightMode = newConfig.uiMode & Configuration.UI_MODE_NIGHT_MASK;
        if (m_CurrentUiMode != currentNightMode) {
            m_CurrentUiMode = currentNightMode;
            configurationChanged = true;

            switch (m_CurrentUiMode) {
                case Configuration.UI_MODE_NIGHT_NO:
                    Log.d("APP UI", "Light Mode enabled");
                    break;
                case Configuration.UI_MODE_NIGHT_YES:
                    Log.d("APP UI", "Dark Mode enabled");
                    break;
                case Configuration.UI_MODE_NIGHT_UNDEFINED:
                    Log.d("APP UI", "Dark Mode is Undefined");
                    break;
            }
        }

        float currentFontScale = newConfig.fontScale;
        if (m_CurrentFontScale != currentFontScale) {
            m_CurrentFontScale = currentFontScale;
            configurationChanged = true;
            Log.d("APP UI", "Changed Font Scale : " + m_CurrentFontScale);
        }

        float currentScaledDensity = displayMetrics.scaledDensity;
        if (m_CurrentScaledDensity != currentScaledDensity) {
            m_CurrentScaledDensity = currentScaledDensity;
            configurationChanged = true;
            Log.d("APP UI", "Changed Scaled Density : " + m_CurrentScaledDensity);
        }

        float currentDensity = displayMetrics.density;
        if (m_CurrentDensity != currentDensity) {
            m_CurrentDensity = currentDensity;
            configurationChanged = true;
            Log.d("APP UI", "Changed Density : " + m_CurrentDensity);
        }

        int currentDensityDpi = newConfig.densityDpi;
        if (m_CurrentDensityDpi != currentDensityDpi) {
            m_CurrentDensityDpi = currentDensityDpi;
            configurationChanged = true;
            Log.d("APP UI", "Changed Density DPI : " + m_CurrentDensityDpi);
        }

        int currentLayoutDirection = GetLayoutDirection(newConfig);
        if (currentLayoutDirection != m_CurrentLayoutDirection) {
            m_CurrentLayoutDirection = currentLayoutDirection;
            configurationChanged = true;
            Log.d("APP UI", "Changed Layout Direction : " + m_CurrentLayoutDirection);
        }

        boolean currentHighContrast = IsHighContrastEnabled(this);
        if (currentHighContrast != m_CurrentHighContrast) {
            m_CurrentHighContrast = currentHighContrast;
            configurationChanged = true;
            Log.d("APP UI", "Changed High Contrast : " + m_CurrentHighContrast);
        }

        if (configurationChanged) {
            UnityPlayer.UnitySendMessage("AppUIUpdater", "OnNativeMessageReceived", "configurationChanged");
        }
    }
}

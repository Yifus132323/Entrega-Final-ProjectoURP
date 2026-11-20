using UnityEngine;

public class AudioActivador : MonoBehaviour
{
    public void playGameplayMusic()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.playGameplayMusic();
        }
    }

    public void playNavigating()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.playNavigating();
        }
    }

    public void playBeep()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.playBeep();
        }
    }

    public void playDiving()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.playDiving();
        }
    }

    

    

    public void StopNavigating()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopNavigating();
        }
    }

    public void StopBeep()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBeep();
        }
    }

    public void StopDiving()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopDiving();
        }
    }

    public void playPick()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.playPick();
        }
    }

    public void StopPick()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopPick();
        }
    }

    public void playDrop()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.playDrop();
        }
    }

    public void StopDrop()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopDrop();
        }
    }

    public void playAlarm()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.playAlarm();
        }
    }

    public void StopAlarm()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopAlarm();
        }
    }

    public void playMenuStartMusic()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.playMenuStartMusic();
        }
    }

    public void StopMenuStartMusic()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMenuStartMusic();
        }
    }




}

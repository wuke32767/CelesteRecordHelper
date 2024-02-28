using Celeste.Mod.Core;
using FMOD;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Celeste.Audio;

namespace Celeste.Mod.RecordHelper.Entities
{
    //Impossible. 

    //public class RecordedAudio:Component,RecordComponemtBase
    //{
    //    Dictionary<string, Recorder<RecordedAudio>> dic = new();
    //    public RecordedAudio() : base(true, false)
    //    {
            
    //    }
    //    public void DelayedUpdate()
    //    {
    //        base.Update();
    //    }

    //    public override void Update()
    //    {
    //    }
    //    #region forward-field
    //    public ref Dictionary<string, EventDescription> cachedEventDescriptions => ref Audio.cachedEventDescriptions;
    //    public ref string CurrentMusic => ref Audio.CurrentMusic;

    //    public EventInstance CurrentMusicEventInstance => Audio.CurrentMusicEventInstance;

    //    public EventInstance CurrentAmbienceEventInstance => Audio.CurrentAmbienceEventInstance;
    //    public float MusicVolume => Audio.MusicVolume;
    //    public float SfxVolume => Audio.SfxVolume;
    //    public bool PauseMusic => Audio.PauseMusic;
    //    public bool PauseGameplaySfx => Audio.PauseGameplaySfx;
    //    public bool PauseUISfx => Audio.PauseUISfx;
    //    public bool MusicUnderwater => Audio.MusicUnderwater;
    //    public FMOD.Studio.System System => Audio.System;
    //    public bool AudioInitialized => Audio.AudioInitialized;


    //    public ref Dictionary<Guid, string> cachedPaths => ref Audio.cachedPaths;

    //    public ref Dictionary<Guid, string> cachedBankPaths => ref Audio.cachedBankPaths;

    //    public ref Dictionary<string, EventDescription> cachedModEvents => ref Audio.cachedModEvents;

    //    #endregion


    //    #region wip
    //    //public static void Init()
    //    //{
    //    //    bool launchWithFMODLiveUpdate = Settings.Instance.LaunchWithFMODLiveUpdate;
    //    //    Settings.Instance.LaunchWithFMODLiveUpdate |= CoreModule.Settings.LaunchWithFMODLiveUpdateInEverest;
    //    //    orig_Init();
    //    //    Settings.Instance.LaunchWithFMODLiveUpdate = launchWithFMODLiveUpdate;
    //    //    Banks.Master = Banks.Load("Master Bank", loadStrings: true);
    //    //    Banks.Music = Banks.Load("music", loadStrings: false);
    //    //    Banks.Sfxs = Banks.Load("sfx", loadStrings: false);
    //    //    Banks.UI = Banks.Load("ui", loadStrings: false);
    //    //    Banks.DlcMusic = Banks.Load("dlc_music", loadStrings: false);
    //    //    Banks.DlcSfxs = Banks.Load("dlc_sfx", loadStrings: false);
    //    //    foreach (Bank value in Banks.Banks.Values)
    //    //    {
    //    //        value.getEventList(out var array);
    //    //        EventDescription[] array2 = array;
    //    //        foreach (EventDescription eventDescription in array2)
    //    //        {
    //    //            if (eventDescription.isValid())
    //    //            {
    //    //                eventDescription.getID(out var id);
    //    //                eventDescription.getPath(out var path);
    //    //                cachedPaths[id] = path;
    //    //                usedGuids[path] = new HashSet<Guid> { id };
    //    //            }
    //    //        }
    //    //    }

    //    //    lock (Everest.Content.Map)
    //    //    {
    //    //        foreach (ModAsset item in Everest.Content.Map.Values.Where((ModAsset asset) => asset.Type == typeof(AssetTypeBank)))
    //    //        {
    //    //            if (!ingestedModBankPaths.Contains(item.PathVirtual))
    //    //            {
    //    //                IngestBank(item);
    //    //            }
    //    //        }
    //    //    }

    //    //    AudioInitialized = true;
    //    //}
    //    //public void Update()
    //    //{
    //    //    if (system != null && ready)
    //    //    {
    //    //        CheckFmod(system.update());
    //    //    }
    //    //}

    //    //public void Unload()
    //    //{
    //    //    if (!(system == null))
    //    //    {
    //    //        system.unloadAll();
    //    //        system.release();
    //    //        system = null;
    //    //        ready = false;
    //    //    }
    //    //}

    //    //public void SetListenerPosition(Vector3 forward, Vector3 up, Vector3 position)
    //    //{
    //    //    FMOD.Studio._3D_ATTRIBUTES attributes = default;
    //    //    attributes.forward.x = forward.X;
    //    //    attributes.forward.z = forward.Y;
    //    //    attributes.forward.z = forward.Z;
    //    //    attributes.up.x = up.X;
    //    //    attributes.up.y = up.Y;
    //    //    attributes.up.z = up.Z;
    //    //    attributes.position.x = position.X;
    //    //    attributes.position.y = position.Y;
    //    //    attributes.position.z = position.Z;
    //    //    system.setListenerAttributes(0, attributes);
    //    //}

    //    //public void SetCamera(Camera camera)
    //    //{
    //    //    currentCamera = camera;
    //    //}

    //    //internal static void CheckFmod(RESULT result)
    //    //{
    //    //    if (result != 0)
    //    //    {
    //    //        throw new Exception($"FMOD Failed: {result} ({Error.String(result)})");
    //    //    }
    //    //}

    //    public EventInstance Play(string path)
    //    {
    //        EventInstance eventInstance = CreateInstance(path);
    //        if (eventInstance != null)
    //        {
    //            eventInstance.start();
    //            eventInstance.release();
    //        }

    //        return eventInstance;
    //    }

    //    public EventInstance Play(string path, string param, float value)
    //    {
    //        EventInstance eventInstance = CreateInstance(path);
    //        if (eventInstance != null)
    //        {
    //            SetParameter(eventInstance, param, value);
    //            eventInstance.start();
    //            eventInstance.release();
    //        }

    //        return eventInstance;
    //    }

    //    public EventInstance Play(string path, Vector2 position)
    //    {
    //        EventInstance eventInstance = CreateInstance(path, position);
    //        if (eventInstance != null)
    //        {
    //            eventInstance.start();
    //            eventInstance.release();
    //        }

    //        return eventInstance;
    //    }

    //    public EventInstance Play(string path, Vector2 position, string param, float value)
    //    {
    //        EventInstance eventInstance = CreateInstance(path, position);
    //        if (eventInstance != null)
    //        {
    //            if (param != null)
    //            {
    //                eventInstance.setParameterValue(param, value);
    //            }

    //            eventInstance.start();
    //            eventInstance.release();
    //        }

    //        return eventInstance;
    //    }

    //    public EventInstance Play(string path, Vector2 position, string param, float value, string param2, float value2)
    //    {
    //        EventInstance eventInstance = CreateInstance(path, position);
    //        if (eventInstance != null)
    //        {
    //            if (param != null)
    //            {
    //                eventInstance.setParameterValue(param, value);
    //            }

    //            if (param2 != null)
    //            {
    //                eventInstance.setParameterValue(param2, value2);
    //            }

    //            eventInstance.start();
    //            eventInstance.release();
    //        }

    //        return eventInstance;
    //    }

    //    public EventInstance Loop(string path)
    //    {
    //        EventInstance eventInstance = CreateInstance(path);
    //        if (eventInstance != null)
    //        {
    //            eventInstance.start();
    //        }

    //        return eventInstance;
    //    }

    //    public EventInstance Loop(string path, string param, float value)
    //    {
    //        EventInstance eventInstance = CreateInstance(path);
    //        if (eventInstance != null)
    //        {
    //            eventInstance.setParameterValue(param, value);
    //            eventInstance.start();
    //        }

    //        return eventInstance;
    //    }

    //    public EventInstance Loop(string path, Vector2 position)
    //    {
    //        EventInstance eventInstance = CreateInstance(path, position);
    //        if (eventInstance != null)
    //        {
    //            eventInstance.start();
    //        }

    //        return eventInstance;
    //    }

    //    public EventInstance Loop(string path, Vector2 position, string param, float value)
    //    {
    //        EventInstance eventInstance = CreateInstance(path, position);
    //        if (eventInstance != null)
    //        {
    //            eventInstance.setParameterValue(param, value);
    //            eventInstance.start();
    //        }

    //        return eventInstance;
    //    }

    //    public void Pause(EventInstance instance)
    //    {
    //        if (instance != null)
    //        {
    //            instance.setPaused(paused: true);
    //        }
    //    }

    //    public void Resume(EventInstance instance)
    //    {
    //        if (instance != null)
    //        {
    //            instance.setPaused(paused: false);
    //        }
    //    }

    //    public void Position(EventInstance instance, Vector2 position)
    //    {
    //        if (instance != null)
    //        {
    //            Vector2 vector = Vector2.Zero;
    //            if (currentCamera != null)
    //            {
    //                vector = currentCamera.Position + new Vector2(320f, 180f) / 2f;
    //            }

    //            float num = position.X - vector.X;
    //            if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
    //            {
    //                num = 0f - num;
    //            }

    //            attributes3d.position.x = num;
    //            attributes3d.position.y = position.Y - vector.Y;
    //            attributes3d.position.z = 0f;
    //            instance.set3DAttributes(attributes3d);
    //        }
    //    }

    //    public void SetParameter(EventInstance instance, string param, float value)
    //    {
    //        if (instance != null)
    //        {
    //            instance.setParameterValue(param, value);
    //        }
    //    }

    //    public void Stop(EventInstance instance, bool allowFadeOut = true)
    //    {
    //        if (instance != null)
    //        {
    //            instance.stop((!allowFadeOut) ? STOP_MODE.IMMEDIATE : STOP_MODE.ALLOWFADEOUT);
    //            instance.release();
    //        }
    //    }

    //    public EventInstance CreateInstance(string path, Vector2? position = null)
    //    {
    //        EventDescription eventDescription = GetEventDescription(path);
    //        if (eventDescription != null)
    //        {
    //            eventDescription.createInstance(out var instance);
    //            eventDescription.is3D(out var is3D);
    //            if (is3D && position.HasValue)
    //            {
    //                Position(instance, position.Value);
    //            }

    //            return instance;
    //        }

    //        return null;
    //    }

    //    public EventDescription GetEventDescription(string path)
    //    {
    //        EventDescription _event = null;
    //        if (path == null || cachedEventDescriptions.TryGetValue(path, out _event))
    //        {
    //            return _event;
    //        }

    //        RESULT rESULT = ((!cachedModEvents.TryGetValue(path, out _event)) ? ((!path.StartsWith("guid://")) ? system.getEvent(path, out _event) : system.getEventByID(new Guid(path.Substring(7)), out _event)) : RESULT.OK);
    //        switch (rESULT)
    //        {
    //            case RESULT.OK:
    //                _event.loadSampleData();
    //                cachedEventDescriptions.Add(path, _event);
    //                break;
    //            case RESULT.ERR_EVENT_NOTFOUND:
    //                if ((!(path == "null") && !(path == "event:/none")) || 1 == 0)
    //                {
    //                    Logger.Log(LogLevel.Warn, "Audio", "Event not found: " + path);
    //                }

    //                break;
    //            default:
    //                throw new Exception("FMOD getEvent failed: " + rESULT);
    //        }
    //        return _event;
    //    }

    //    public void ReleaseUnusedDescriptions()
    //    {
    //        if (CoreModule.Settings.UnloadUnusedAudio)
    //        {
    //            orig_ReleaseUnusedDescriptions();
    //        }
    //    }

    //    public string GetEventName(EventInstance instance)
    //    {
    //        if (instance == null)
    //        {
    //            return "";
    //        }

    //        instance.getDescription(out var description);
    //        if (description == null)
    //        {
    //            return "";
    //        }

    //        return GetEventName(description);
    //    }

    //    public bool IsPlaying(EventInstance instance)
    //    {
    //        if (instance != null)
    //        {
    //            instance.getPlaybackState(out var state);
    //            if (state == PLAYBACK_STATE.PLAYING || state == PLAYBACK_STATE.STARTING)
    //            {
    //                return true;
    //            }
    //        }

    //        return false;
    //    }

    //    public bool BusPaused(string path, bool? pause = null)
    //    {
    //        bool paused = false;
    //        if (system != null && system.getBus(path, out var bus) == RESULT.OK)
    //        {
    //            if (pause.HasValue)
    //            {
    //                bus.setPaused(pause.Value);
    //            }

    //            bus.getPaused(out paused);
    //        }

    //        return paused;
    //    }

    //    public bool BusMuted(string path, bool? mute)
    //    {
    //        bool paused = false;
    //        if (system.getBus(path, out var bus) == RESULT.OK)
    //        {
    //            if (mute.HasValue)
    //            {
    //                bus.setMute(mute.Value);
    //            }

    //            bus.getPaused(out paused);
    //        }

    //        return paused;
    //    }

    //    public void BusStopAll(string path, bool immediate = false)
    //    {
    //        if (system != null && system.getBus(path, out var bus) == RESULT.OK)
    //        {
    //            bus.stopAllEvents(immediate ? STOP_MODE.IMMEDIATE : STOP_MODE.ALLOWFADEOUT);
    //        }
    //    }

    //    public float VCAVolume(string path, float? volume = null)
    //    {
    //        VCA vca;
    //        RESULT vCA = system.getVCA(path, out vca);
    //        float volume2 = 1f;
    //        float finalvolume = 1f;
    //        if (vCA == RESULT.OK)
    //        {
    //            if (volume.HasValue)
    //            {
    //                vca.setVolume(volume.Value);
    //            }

    //            vca.getVolume(out volume2, out finalvolume);
    //        }

    //        return volume2;
    //    }

    //    public EventInstance CreateSnapshot(string name, bool start = true)
    //    {
    //        system.getEvent(name, out var _event);
    //        if (_event == null)
    //        {
    //            throw new Exception("Snapshot " + name + " doesn't exist");
    //        }

    //        _event.createInstance(out var instance);
    //        if (start)
    //        {
    //            instance.start();
    //        }

    //        return instance;
    //    }

    //    public void ResumeSnapshot(EventInstance snapshot)
    //    {
    //        if (snapshot != null)
    //        {
    //            snapshot.start();
    //        }
    //    }

    //    public bool IsSnapshotRunning(EventInstance snapshot)
    //    {
    //        if (snapshot != null)
    //        {
    //            snapshot.getPlaybackState(out var state);
    //            if (state != 0 && state != PLAYBACK_STATE.STARTING)
    //            {
    //                return state == PLAYBACK_STATE.SUSTAINING;
    //            }

    //            return true;
    //        }

    //        return false;
    //    }

    //    public void EndSnapshot(EventInstance snapshot)
    //    {
    //        if (snapshot != null)
    //        {
    //            snapshot.stop(STOP_MODE.ALLOWFADEOUT);
    //        }
    //    }

    //    public void ReleaseSnapshot(EventInstance snapshot)
    //    {
    //        if (snapshot != null)
    //        {
    //            snapshot.stop(STOP_MODE.ALLOWFADEOUT);
    //            snapshot.release();
    //        }
    //    }

    //    public bool SetMusic(string path, bool startPlaying = true, bool allowFadeOut = true)
    //    {
    //        if (string.IsNullOrEmpty(path) || path == "null")
    //        {
    //            Stop(currentMusicEvent, allowFadeOut);
    //            currentMusicEvent = null;
    //            CurrentMusic = "";
    //        }
    //        else if (!CurrentMusic.Equals(path, StringComparison.OrdinalIgnoreCase))
    //        {
    //            Stop(currentMusicEvent, allowFadeOut);
    //            EventInstance eventInstance = CreateInstance(path);
    //            if (eventInstance != null && startPlaying)
    //            {
    //                eventInstance.start();
    //            }

    //            currentMusicEvent = eventInstance;
    //            CurrentMusic = GetEventName(eventInstance);
    //            return true;
    //        }

    //        return false;
    //    }

    //    public bool SetAmbience(string path, bool startPlaying = true)
    //    {
    //        if (string.IsNullOrEmpty(path) || path == "null")
    //        {
    //            Stop(currentAmbientEvent);
    //            currentAmbientEvent = null;
    //        }
    //        else if (!GetEventName(currentAmbientEvent).Equals(path, StringComparison.OrdinalIgnoreCase))
    //        {
    //            Stop(currentAmbientEvent);
    //            EventInstance eventInstance = CreateInstance(path);
    //            if (eventInstance != null && startPlaying)
    //            {
    //                eventInstance.start();
    //            }

    //            currentAmbientEvent = eventInstance;
    //            return true;
    //        }

    //        return false;
    //    }

    //    public void SetMusicParam(string path, float value)
    //    {
    //        if (currentMusicEvent != null)
    //        {
    //            currentMusicEvent.setParameterValue(path, value);
    //        }
    //    }

    //    public void SetAltMusic(string path)
    //    {
    //        if (string.IsNullOrEmpty(path))
    //        {
    //            EndSnapshot(mainDownSnapshot);
    //            Stop(currentAltMusicEvent);
    //            currentAltMusicEvent = null;
    //        }
    //        else if (!GetEventName(currentAltMusicEvent).Equals(path, StringComparison.OrdinalIgnoreCase))
    //        {
    //            StartMainDownSnapshot();
    //            Stop(currentAltMusicEvent);
    //            currentAltMusicEvent = Loop(path);
    //        }
    //    }

    //    private static void StartMainDownSnapshot()
    //    {
    //        if (mainDownSnapshot == null)
    //        {
    //            mainDownSnapshot = CreateSnapshot("snapshot:/music_mains_mute");
    //        }
    //        else
    //        {
    //            ResumeSnapshot(mainDownSnapshot);
    //        }
    //    }

    //    private static void EndMainDownSnapshot()
    //    {
    //        EndSnapshot(mainDownSnapshot);
    //    }
    //    #endregion

    //    #region forward-method
    //    public static void orig_Init() => Audio.orig_Init();
    //    public static void IngestNewBanks() => Audio.IngestNewBanks();
    //    public static Bank IngestBank(ModAsset asset) => Audio.IngestBank(asset);
    //    public static void IngestGUIDs(ModAsset asset) => Audio.IngestGUIDs(asset);
    //    public static void orig_ReleaseUnusedDescriptions() => Audio.orig_ReleaseUnusedDescriptions();
    //    public static string GetEventName(EventDescription desc) => Audio.GetEventName(desc);
    //    public static string GetBankName(Bank bank) => Audio.GetBankName(bank);
    //    #endregion
    //}
}

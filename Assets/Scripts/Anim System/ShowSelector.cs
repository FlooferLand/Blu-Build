using System;

[Serializable]
public class ShowTapeSelector {
    public string showtapeName;
    public string showtapeDate;
    public ShowtapeSegment[] segments;
}

[Serializable]
public class ShowtapeSegment {
    public enum AudioQ {
        None,
        Unobtained,
        Incomplete,
        Corrupted,
        LowQuality,
        ReelQuality,
        MasteredAudio
    }

    public enum ShowQ {
        Original,
        Edited,
        Duplicate,
        DuplicateEdited,
        NewSignals,
        DuplicateNewSignals
    }

    public enum SignalQ {
        None,
        Unobtained,
        Incomplete,
        Corrupted,
        Complete
    }

    public enum VideoQ {
        None,
        Unobtained,
        Incomplete,
        Corrupted,
        LowQuality,
        ReelQuality,
        MasteredVideo
    }

    public SignalQ signalQuality = SignalQ.Complete;
    public AudioQ audioQuality = AudioQ.ReelQuality;
    public VideoQ videoQuality = VideoQ.None;
    public ShowQ showVersion;
    public string segmentNumber;
    public string showName;
    public string showLength;
}
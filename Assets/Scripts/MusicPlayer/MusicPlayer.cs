using UnityEngine;

public class MusicPlayer : MonoBehaviour {
    // 1. Array of music tracks to play
    public AudioClip[] musicClips;

    // 2. The AudioSource component
    private AudioSource audioSource;

    // 3. Keep track of the last played clip index
    private int lastClipIndex = -1;

    void Start() {
        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();

        // Make sure we have an AudioSource
        if (audioSource == null) {
            Debug.LogError("No AudioSource component found. Please add one.");
            return;
        }

        // Make sure we have clips to play
        if (musicClips.Length == 0) {
            Debug.LogWarning("No music clips assigned to the MusicPlayer.");
            return;
        }

        // Start playing the first random track
        PlayRandomMusic();
    }

    void Update() {
        // 4. Check if the current clip has finished playing
        if (audioSource != null && !audioSource.isPlaying && musicClips.Length > 0) {
            // If it's finished, play the next random one
            PlayRandomMusic();
        }
    }

    void PlayRandomMusic() {
        // 5. Pick a new random index
        int randomIndex = Random.Range(0, musicClips.Length);

        // 6. Basic check to avoid playing the same song twice in a row
        if (musicClips.Length > 1) {
            while (randomIndex == lastClipIndex) {
                randomIndex = Random.Range(0, musicClips.Length);
            }
        }

        // Update the last played index
        lastClipIndex = randomIndex;

        // 7. Assign and play the new clip
        audioSource.clip = musicClips[randomIndex];
        audioSource.Play();
    }
}
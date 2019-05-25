using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour {

    public AudioClip[] destroyBlockSound;
    public AudioClip[] placeBlockSounds;

    // play the destroy block sound
    void PlaceBlockSound(int blockType, bool voxelDestroyed)
    {
        if (!voxelDestroyed)
        {
            switch (blockType)
            {
                case 1:
                    GetComponent<AudioSource>().PlayOneShot(placeBlockSounds[blockType - 1]);
                    break;
                case 2:
                    GetComponent<AudioSource>().PlayOneShot(placeBlockSounds[blockType - 1]);
                    break;
                case 3:
                    GetComponent<AudioSource>().PlayOneShot(placeBlockSounds[blockType - 1]);
                    break;
                case 4:
                    GetComponent<AudioSource>().PlayOneShot(placeBlockSounds[blockType - 1]);
                    break;
            }
        }

        if(voxelDestroyed)
        {
            switch (blockType)
            {
                case 1:
                    GetComponent<AudioSource>().PlayOneShot(destroyBlockSound[blockType - 1]);
                    break;
                case 2:
                    GetComponent<AudioSource>().PlayOneShot(destroyBlockSound[blockType - 1]);
                    break;
                case 3:
                    GetComponent<AudioSource>().PlayOneShot(destroyBlockSound[blockType - 1]);
                    break;
                case 4:
                    GetComponent<AudioSource>().PlayOneShot(destroyBlockSound[blockType - 1]);
                    break;
            }
        }
    }

    // When game object is enabled
    void OnEnable()
    {
        VoxelChunk.OnEventBlockChanged += PlaceBlockSound;
    }
    // When game object is disabled
    void OnDisable()
    {
        VoxelChunk.OnEventBlockChanged -= PlaceBlockSound;
    }
}

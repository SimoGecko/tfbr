# Audio

The sound library is located in `BRS/Content/Audio`, where the main sources are organized in the `songs` and `effects` folders. All the sound effects are converted 
in mono wmv format. The whole library can be found in the group shared folder `sounds/effects_mono` and `sounds/effects_mono_sorted`. The sound library can be
enriched.

Currently sounds are added in the game at appropriate places, including background music, power-up pickup (currently only key and bomb are implemented, except this two, all 
the other power-ups are using the same pickup sound), use power-up(same as pickup), attacking, crack crates and collision. More sounds can be added if needed.

Currently the player position is used as `Vector3.Zero`, when using object positions the volume are sometimes low.

Pipeline error exists.
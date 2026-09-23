import mido
import csv
import sys
import os
from collections import defaultdict

# Check for command-line argument
if len(sys.argv) < 2:
    print("Usage: python drums.py <filename>.mid")
    sys.exit(1)

input_filename = sys.argv[1]

# Check if the file exists
if not os.path.isfile(input_filename):
    print(f"File '{input_filename}' not found.")
    sys.exit(1)

# Define drum note ranges (MIDI notes 35-81 are standard percussion)
DRUM_NOTE_RANGE = range(35, 82)  # From Acoustic Bass Drum to Open Triangle

# Load MIDI file
mid = mido.MidiFile(input_filename)
tpqn = mid.ticks_per_beat

# Collect all events with absolute tick positions
events = []
for track in mid.tracks:
    tick_accumulator = 0
    for msg in track:
        tick_accumulator += msg.time
        events.append((tick_accumulator, msg))
events.sort(key=lambda x: x[0])

# Process events with tempo handling
current_tempo = 500000  # default 120 BPM
current_time = 0.0
last_tick = 0
notes = []
tempo_changes = []

for tick, msg in events:
    # Calculate delta ticks since last event
    delta_ticks = tick - last_tick
    
    # Convert delta ticks to seconds using current tempo
    delta_seconds = delta_ticks * (current_tempo / 1_000_000) / tpqn
    current_time += delta_seconds
    
    # Update last tick position
    last_tick = tick
    
    # Handle tempo changes
    if msg.type == 'set_tempo':
        current_tempo = msg.tempo
        tempo_changes.append((current_time, current_tempo))
    
    # Record only drum notes (within specified range)
    if (msg.type == 'note_on' and 
        msg.velocity > 0 and 
        msg.note in DRUM_NOTE_RANGE):
        notes.append({
            "Name": f"Note{len(notes)}",
            "time": round(current_time, 4),
            "note": msg.note,
            "velocity": msg.velocity,
            "channel": msg.channel
        })

# Calculate full song duration including trailing silence
total_ticks = max(tick for tick, _ in events)
total_seconds = total_ticks * (current_tempo / 1_000_000) / tpqn

# Create output filename
base_name = os.path.splitext(input_filename)[0]
output_filename = f"{base_name}_note_times.csv"

# Write to CSV
with open(output_filename, "w", newline="") as f:
    writer = csv.DictWriter(f, fieldnames=["Name", "time", "note", "velocity", "channel"])
    writer.writeheader()
    writer.writerows(notes)

# Debug output
if notes:
    print(f"Output written to {output_filename}")
    print(f"Last drum note time: {notes[-1]['time']} seconds")
    print(f"Full song duration: {total_seconds:.2f} seconds")
    print(f"Total drum notes: {len(notes)}")
    print(f"Tempo changes: {len(tempo_changes)}")
    if tempo_changes:
        print(f"First tempo: {tempo_changes[0][1]} at {tempo_changes[0][0]}s")
        print(f"Last tempo: {tempo_changes[-1][1]} at {tempo_changes[-1][0]}s")
else:
    print("No drum notes found in MIDI file")

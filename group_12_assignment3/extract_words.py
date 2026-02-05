import re
from collections import defaultdict
from pathlib import Path

def extract_words(text: str) -> list[str]:
    # Lowercase first, then extract only alphabetic sequences
    text = text.lower()
    return re.findall(r"[a-z]+", text)

def main():
    # Adjust input filename as needed
    input_path = Path("novel.txt")
    if not input_path.exists():
        raise FileNotFoundError("Could not find novel.txt in this folder.")

    raw = input_path.read_text(encoding="utf-8", errors="ignore")
    words = extract_words(raw)

    # allwords.txt
    Path("allwords.txt").write_text("\n".join(words), encoding="utf-8")

    # Count word occurrences
    counts = defaultdict(int)
    for w in words:
        counts[w] += 1

    # uniquewords.txt: words that appear exactly once
    unique_words = [w for w, c in counts.items() if c == 1]
    unique_words.sort()
    Path("uniquewords.txt").write_text("\n".join(unique_words), encoding="utf-8")

    # wordfrequency.txt: frequency -> how many words have that frequency
    freq_counts = defaultdict(int)
    for c in counts.values():
        freq_counts[c] += 1

    lines = []
    for freq in sorted(freq_counts.keys()):
        lines.append(f"{freq}: {freq_counts[freq]}")
    Path("wordfrequency.txt").write_text("\n".join(lines), encoding="utf-8")

    print("Wrote allwords.txt, uniquewords.txt, wordfrequency.txt")

if __name__ == "__main__":
    main()

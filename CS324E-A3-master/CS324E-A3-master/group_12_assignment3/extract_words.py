import re
from collections import defaultdict

def extract_words(text):
    # convert to lowercase
    text = text.lower()

    # extract alphabetic words only
    words = re.findall(r"[a-z]+", text)

    return words

def main():
    # open novel file
    try:
        file = open("Content/novel.txt", "r", encoding="utf-8", errors="ignore")
    except FileNotFoundError:
        print("Could not find novel.txt")
        return

    raw_text = file.read()
    file.close()

    # extract words
    words = extract_words(raw_text)

    # allwords.txt
    outfile = open("allwords.txt", "w")

    for w in words:
        outfile.write(w + "\n")

    outfile.close()

    # count occurrences
    counts = defaultdict(int)

    for w in words:
        counts[w] += 1

    # uniquewords.txt
    outfile = open("uniquewords.txt", "w")

    for w in counts:
        if counts[w] == 1:
            outfile.write(w + "\n")

    outfile.close()

    # wordfrequency.txt
    freq_counts = defaultdict(int)

    for w in counts:
        freq = counts[w]
        freq_counts[freq] += 1

    outfile = open("wordfrequency.txt", "w")

    freqs = list(freq_counts.keys())
    freqs.sort()

    for f in freqs:
        line = str(f) + ": " + str(freq_counts[f])
        outfile.write(line + "\n")

    outfile.close()

    print("Finished writing output files.")

if __name__ == "__main__":
    main()

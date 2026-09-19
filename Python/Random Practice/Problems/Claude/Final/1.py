# Problem 1

# Given a sentence, count the frequency of each word and print the most frequent word. If tie, print alphabetically smallest.
# Input: "the cat sat on the mat the cat"

# Output: cat



from collections import Counter
def fre(str):
    arr = str.split(" ")
    freq = Counter(arr)
    if len(freq) > 2 and freq.most_common(1)[0][1] == freq.most_common(2)[1][1]:
        smallest = freq.most_common(1)[0][0] 
        for i,j in freq.most_common():
            # print(f"i:{i}, h:{j} smallest:{smallest} common:{freq.most_common(1)[0][1]}")
            if j != freq.most_common(1)[0][1]:
                break
            smallest = min(smallest, i )
        return smallest    
    return freq.most_common(1)[0][0]


print(fre("the cat sat on the mat cat the cat a a a a cat"))


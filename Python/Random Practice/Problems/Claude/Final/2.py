# Given a list of integers, return a new list with duplicates removed but original order preserved.
# Input: [3, 1, 2, 1, 3, 4]

# Output: [3, 1, 2, 4]

from collections import Counter
def remDuplicate(num):
    fre = Counter(num)
    num.clear()
    for i in fre:
        num.append(i)
    return num    

for i in range(100):
    print(remDuplicate([3, 1, 2, 1, 3, 4]))    
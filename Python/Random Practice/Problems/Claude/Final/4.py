# Problem 4

# Given a list of integers, find all elements that appear more than once.
# Input: [1, 2, 3, 2, 4, 3, 5]

# Output: [2, 3]

from collections import Counter
def appearMoreThanOnce(num):
    fre = Counter(num).most_common()
    output = []
    for i,j in fre:
        if j > 1:
            output.append(i)
    return output      
print(appearMoreThanOnce([1, 2, 3, 2, 4, 3, 5]))  
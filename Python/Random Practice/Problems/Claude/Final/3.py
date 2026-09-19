# Problem 3

# Given a string, check if two strings are anagrams of each other.
# Input: "listen" and "silent"

# Output: True

from collections import Counter
def areAnagments(str1, str2):
    fre1 = Counter(str1).most_common()
    fre2 = Counter(str2).most_common()
    fre1.sort()
    fre2.sort()
    for index,i in enumerate(fre1):
        if i != fre2[index]:
            return False
    return True    
    
def areAnagments2(stra, strb):
    
    str1 = sorted(stra)
    str2 = sorted(strb)
    for index,i in enumerate(str1):
        if i != str2[index]:
            return False
    return True        

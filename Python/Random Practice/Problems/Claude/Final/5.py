# Problem 5

# Given a dictionary of student names and scores, print names of all students who scored above the class average.
# Input:
# {"Ali": 80, "Sara": 90, "John": 70, "Zara": 85}
# Output:
# Sara
# Zara

def aboveAverage(dict):
    sum = 0
    for i in dict.values():
        sum += i
    ave = sum / len(dict)
    for i,j in dict.items():
        if j > ave:
            print(i)


aboveAverage({"Ali": 80, "Sara": 90, "John": 70, "Zara": 85})            

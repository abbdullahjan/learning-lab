def largest(num):
    maximum = max(num)
    repitition = 0
    for i in num:
        if maximum == i:
            repitition += 1
    if repitition > 1:
        return min(num)
    return maximum

print(largest([3, 1, 2, 3, 1, 3, 2, 1]))        
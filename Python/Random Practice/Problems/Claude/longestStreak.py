def longestStreak(num):
    num.sort()
    longest = 0
    pre = 0
    if len(num) > 0:
        pre = num[0]
    currentStreak = 0
    if len(num) > 1:
        if num[0] == (num[1] - 1) :
            currentStreak = 1    
    i = 1
    while i < len(num):        
        if pre == num[i] - 1 :
            currentStreak += 1
            if currentStreak > longest:
                longest = currentStreak
        else:
            currentStreak = 0
        pre = num[i]
        i+=1

    return longest    

print(longestStreak([1,2,3,4]))
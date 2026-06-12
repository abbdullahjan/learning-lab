def largestFrequency(num):
    num.sort()
    hf = 0
    lf = None
    i = 0
    freRepeat = False
    while i < len(num):
        if i > 0 and num[i] == num[i-1]:
            i+=1
            continue
        fre = num.count(num[i])
        if fre > hf:
            hf = fre 
            freRepeat = False
        elif fre == hf:
            freRepeat = True

        if lf is None or fre < lf:
            lf = fre
        i+=1
    if freRepeat:
        return lf
    return hf

print(largestFrequency([1,2,3,3,3,4,4,4]))

        
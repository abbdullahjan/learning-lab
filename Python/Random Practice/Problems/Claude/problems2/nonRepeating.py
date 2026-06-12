def nonRepeating(str):
    index = -1
    for i in str:
        if str.count(i) == 1:
            index = i
            break
    return index

print(nonRepeating("aabb"))    
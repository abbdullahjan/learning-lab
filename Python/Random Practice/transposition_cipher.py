text="abcdefghijklmnopqrstuvwxyz"
size = 4
# lets create asimple program first which transposes the text
def split_len(text, size):
    arr = []
    for i in range(0,len(text),size):
        arr.append(text[i:i+size])

    return arr

def transpose(text, size):
    cipher = ""
    arr = split_len(text, size)
    for i in range(0, size):
        for j in arr:
            if len(j) > i :
                cipher = cipher + j[i]
    return cipher        

print(text)
arr = split_len(text, size)
for i in arr:
    print(i)

print(transpose(text,size))


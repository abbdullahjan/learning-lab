def sumToTarget(arr, target):
    output = []
    i = 0
    while i < len(arr):
        j = i + 1
        while j < len(arr):
            if arr[i] + arr[j] == target:
                output.append((arr[i],arr[j]))
            j+=1    
        i+=1
    return output

print(sumToTarget([1, 5, 3, 2, 4], 6))            

        
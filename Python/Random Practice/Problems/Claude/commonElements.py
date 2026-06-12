def common(num1, num2):
    output = []
    for i in num1:
        if i in num2 and i not in output:
            output.append(i) 
    return output

print(common([1, 2, 2, 3, 4] ,[2, 3, 3, 5]))            
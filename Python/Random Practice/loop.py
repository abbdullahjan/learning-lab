# count = 0
# print("start")
# for var in range(3,1,1):
#     print("hello1")
#     count += 1 
#     if count > 10:
#         break


def main():
    i = 1
    limit = 6
    while i <= limit:
        function2(i,2,limit)
        print(function1(i, 2))
        i += 1
def function1(i, num):
    line = ""
    for j in range(1, i):
        line += str(num) + " "
        num *= 2
    return line

def function2(i, num, limit):
    # print("function2 call")
    line = ""
    highest = 2**limit
    spacingVar = i
    while(spacingVar <= limit):
        # print(f'j={j}, limit={limit}, highest={highest}')
        line = line + (len(str(abs(i))) * " ") + " "
        highest =   highest // 2
        spacingVar *= 2

    print(line, end = "")

    # print(f'highest = {highest}')
    while(highest > 2):
    # for(i = highest , highest > 2 , i = i/2)
        print(highest,"",end="")
        highest //= 2
main()


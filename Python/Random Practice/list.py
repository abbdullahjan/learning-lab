# dictionary
sample_dict = {
    "user1": {
        "name": "Kelly",
        "age": 25,
        "salary": 8000,
        "city": "New York"
    },
    "user2": {
        "name": "Ahmed",
        "age": 30,
        "salary": 9000,
        "city": "London"
    },
    "user3": {
        "name": "Sara",
        "age": 22,
        "salary": 7000,
        "city": "Toronto"
    },
    "user4": {
        "name": "John",
        "age": 28,
        "salary": 8500,
        "city": "Sydney"
    },
    "user5": {
        "name": "Ali",
        "age": 27,
        "salary": 7800,
        "city": "Dubai"
    },
    "user6": {
        "name": "Emma",
        "age": 24,
        "salary": 8200,
        "city": "Berlin"
    },
    "user7": {
        "name": "David",
        "age": 35,
        "salary": 10000,
        "city": "Chicago"
    },
    "user8": {
        "name": "Ayesha",
        "age": 26,
        "salary": 7600,
        "city": "Karachi"
    },
    "user9": {
        "name": "Michael",
        "age": 29,
        "salary": 9500,
        "city": "San Francisco"
    },
    "user10": {
        "name": "Zara",
        "age": 23,
        "salary": 7300,
        "city": "Paris"
    }
}
keys = ["name", "salary"]
users = []
for key in sample_dict:
    users.append(key)
new_dict = {}
for user in users:
    new_dict[user] = {}  
    for k in keys:
        new_dict[user][k] = sample_dict[user][k]

print(new_dict)

# # set

# set1 = {1,4,2}
# set2 = {1,2,4}
# print(f'Set1 = {id(set1)}, Set2 = {id(set2)}')
# print(set1 == set2)


# get remobe duplicate characters from aa string 
# s = list(input("Enter a String: "))
# print(s)
# set1 = set(s)
# requiredList = list(set1)
# print("Duplicates Removed: ",requiredList)
# set1.append(1)

# print(set1)

# tuple

# lst = [1,2,3,4,5,6,7,8,9,10]
# print("Before Tuple: ", lst)
# lst.append([11,12,13])
# print("After Tuple: ", lst)
# # can we change value at tuple?
# # lst[10][1] = 5
# print(lst[10][1])




# t1 = (1,2,3,"abc")
# t2 = (1,2,3,4,5,6,7,8)
# t1 = t2
# print(t1)


# # checking how tuple and list are different
# lst = [1,2,3,4,5,6,7,8,9,0,11,12,13]
# lst.append(14)
# print(lst)
# tup = tuple(lst)
# print(type(tup))
# print(type(lst))
# print(lst[0])
# print(tup[0])
# print(len(lst))
# print(len(tup))
# for i in lst:
#     print(id(i))




# s = input("Enter 10 numbers separated by spaces from one line: ")
# items = s.split() # Extract items from the string
# lst = [eval(x) for x in items] 
# i = 0
# for x in lst:
#     print(i, "= " ,x)
#     i += 1
import numpy as np

data = np.array([
    [101,  20,  65,  72,  1500],
    [102,  21,  70,  68,  1200],
    [103,  19,  55,  81,  1800],
    [104,  22,  80,  75,  2200],
    [105,  20,  60,  90,  2500],
    [106,  23,  75,  64,  1100],
    [107,  21,  68,  78,  1950],
    [108,  24,  85,  70,  2100],
    [109,  19,  50,  88,  1600],
    [110,  22,  72,  83,  2300],
    [111,  20,  62,  76,  1750],
    [112,  23,  90,  69,  2800],
])

# shape of data 
print("Data Shape: ", np.shape(data))

# Number of Rows
print("Number of ROws: ", data.shape[0])

# Number of Columns
print("Number of Columns: ", data.shape[1])

# print first student
print("first Student: ", data[0])


# print last student
print("Last Student: ", data[data.shape[0]-1])

# 7. Print students 3 through 7
print("students 3 through 7: \n", data[2:8])


# 8. Get the age of student 105
# print("Test: ", data[0:3,1])
print("Äge of student 105:", data[data[:,0] == 105,1][0])
import numpy as np

A = np.array([
    [2, 4],
    [1, 3]
])

B = np.array([
    [5, 2],
    [4, 1]
])



print(f'Sum: \n{A + B}')
print(f'Difference: \n{A - B}')
print(f'Element Wise: \n{A * B}')      # element-wise
print(f'Matrix Multiplcation: \n{A @ B}')      # matrix multiplication


# Sorting
unsorted = np.array([[5,1,2],[1,3,1],[8,4,1],[1,9,1]])
print("Original array: \n",unsorted)
print("Sorted array: \n",np.sort(unsorted,axis=0))
print("Sorted array: \n",np.sort(unsorted,axis=1))
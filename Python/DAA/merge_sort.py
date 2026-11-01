"""
Design and Analysis of Algorithms (DAA)
Algorithm 2: MergeSort(A[0..n - 1])

Input:  A[0..n - 1]: unsorted array
Output: A[0..n - 1]: sorted array in ascending order

Pseudocode from Algorithm 2:
----------------------------
1 if n > 1 then
2     copy A[0..⌊n/2⌋ - 1] to B[0..⌊n/2⌋ - 1];
3     copy A[⌊n/2⌋..n - 1] to C[0..⌈n/2⌉ - 1];
4     MergeSort(B[0..⌊n/2⌋ - 1]);
5     MergeSort(C[0..⌈n/2⌉ - 1]);
6     Merge(B, C, A);
7 end
"""

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


def merge(B: list, C: list, A: list) -> None:
    """
    Merge(B[0..p - 1], C[0..q - 1], A[0..p + q - 1])
    Merges two sorted arrays B and C into sorted array A.
    """
    p = len(B)
    q = len(C)
    i = 0
    j = 0
    k = 0

    while i < p and j < q:
        if B[i] <= C[j]:
            A[k] = B[i]
            i += 1
        else:
            A[k] = C[j]
            j += 1
        k += 1

    if i == p:
        # Copy remaining elements of C into A
        while j < q:
            A[k] = C[j]
            j += 1
            k += 1
    else:
        # Copy remaining elements of B into A
        while i < p:
            A[k] = B[i]
            i += 1
            k += 1


def merge_sort(A: list) -> None:
    """
    Sorts array A[0..n - 1] in ascending order using MergeSort.
    Modifies array A in-place according to Algorithm 2.
    """
    n = len(A)
    # Line 1: if n > 1 then
    if n > 1:
        mid = n // 2  # ⌊n/2⌋

        # Line 2: copy A[0..⌊n/2⌋ - 1] to B[0..⌊n/2⌋ - 1]
        B = A[:mid]

        # Line 3: copy A[⌊n/2⌋..n - 1] to C[0..⌈n/2⌉ - 1]
        C = A[mid:]

        # Line 4: MergeSort(B[0..⌊n/2⌋ - 1])
        merge_sort(B)

        # Line 5: MergeSort(C[0..⌈n/2⌉ - 1])
        merge_sort(C)

        # Line 6: Merge(B, C, A)
        merge(B, C, A)

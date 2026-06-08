# Given an integer array nums, rotate the array to the right by k steps, where k is non-negative.

 

# Example 1:

# Input: nums = [1,2,3,4,5,6,7], k = 3
# Output: [5,6,7,1,2,3,4]
# Explanation:
# rotate 1 steps to the right: [7,1,2,3,4,5,6]
# rotate 2 steps to the right: [6,7,1,2,3,4,5]
# rotate 3 steps to the right: [5,6,7,1,2,3,4]

# Example 2:

# Input: nums = [-1,-100,3,99], k = 2
# Output: [3,99,-1,-100]
# Explanation: 
# rotate 1 steps to the right: [99,-1,-100,3]
# rotate 2 steps to the right: [3,99,-1,-100]

 

# Constraints:

#     1 <= nums.length <= 105
#     -231 <= nums[i] <= 231 - 1
#     0 <= k <= 105

nums = [2147483647,-2147483648,33,219,0]
k = 4
class Solution(object):
    def rotate(self,nums, k):
        if len(nums) < 0 or len(nums) > 2**105:
            print("Error")
            return 
        rotated = []
        i = 0
        while i < len(nums):
            if nums[i] < -2147483648  or nums[i] > 2147483647:
                print("Error item length")
                return
            rotated.append(nums[(len(nums) - k + i) % len(nums)])
            i+=1

        nums[:] = rotated    
        return nums    

s = Solution

print(s.rotate(s,nums, k))
        
        
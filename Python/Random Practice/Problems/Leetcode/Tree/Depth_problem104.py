# 104. Maximum Depth of Binary Tree
# Easy
# Topics
# premium lock iconCompanies

# Given the root of a binary tree, return its maximum depth.

# A binary tree's maximum depth is the number of nodes along the longest path from the root node down to the farthest leaf node.

 

# Example 1:

# Input: root = [3,9,20,null,null,15,7]
# Output: 3

# Example 2:

# Input: root = [1,null,2]
# Output: 2

 

# Constraints:

#     The number of nodes in the tree is in the range [0, 104].
#     -100 <= Node.val <= 100


from collections import deque

class TreeNode(object):
    def __init__(self, val=0, left=None, right=None):
        self.val = val
        self.left = left
        self.right = right

    


class Solution(object):
    def maxDepth(self, root):
        if root is None:
            return 0
        s = Solution()
        left = s.maxDepth(root.left)
        right = s.maxDepth(root.right)
        return 1 + max(left, right)
        
mytree = TreeNode()


def buildTree( num ):
        if not num or num[0] is None:
            return None
        root = TreeNode(num[0])
        queue = deque([root])
        i = 1
        while queue  and i < len(num) :
            cur = queue.popleft()  
            
            if i<len(num) and num[i] is not None:
                cur.left = TreeNode(num[i])
                queue.append(cur.left)
            i+=1

            if i<len(num) and num[i] is not None:
                cur.right = TreeNode(num[i])
                queue.append(cur.right)
            i+=1

        return root

mytree = buildTree([1,2,3,4,5])
s = Solution()
print(mytree)
print(s.maxDepth(mytree))

print("...")
def dfs(node):
    print(node.val)
    if node.left is not None:
        dfs(node.left)
    if node.right is not None:
        dfs(node.right)
dfs(mytree)

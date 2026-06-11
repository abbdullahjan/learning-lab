class node:
    def __init__(self, data):
        self.data = data
        self.left = None
        self.right = None

def dfs(root):
    print(root.data)
    if root.left is not None:
        dfs(root.left)
    if root.right is not None:
        dfs(root.right)
    

root = node("root")
myNode = []
for i in range(10):
    myNode.append(node(chr(ord('A') + i)))
root.left = myNode[0]
root.right = myNode[1]

i = 2
j = 0

while i < 10:
    myNode[j].left = myNode[i]
    i+=1
    myNode[j].right = myNode[i]
    i+=1
    j+=1

dfs(root)

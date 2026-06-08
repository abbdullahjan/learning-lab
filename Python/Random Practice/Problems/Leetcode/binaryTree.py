# You are given a 2D integer array descriptions where descriptions[i] = [parenti, childi, isLefti] indicates that parenti is the parent of childi in a binary tree of unique values. Furthermore,

#     If isLefti == 1, then childi is the left child of parenti.
#     If isLefti == 0, then childi is the right child of parenti.

# Construct the binary tree described by descriptions and return its root.

# The test cases will be generated such that the binary tree is valid.

 

# Example 1:

# Input: descriptions = [[20,15,1],[20,17,0],[50,20,1],[50,80,0],[80,19,1]]
# Output: [50,20,80,15,17,19]
# Explanation: The root node is the node with value 50 since it has no parent.
# The resulting binary tree is shown in the diagram.

# Example 2:

# Input: descriptions = [[1,2,1],[2,3,0],[3,4,1]]
# Output: [1,2,null,null,3,4]
# Explanation: The root node is the node with value 1 since it has no parent.
# The resulting binary tree is shown in the diagram.

 

# Constraints:

#     1 <= descriptions.length <= 104
#     descriptions[i].length == 3
#     1 <= parenti, childi <= 105
#     0 <= isLefti <= 1
#     The binary tree described by descriptions is valid



description = [[1,2,1],[2,3,0],[3,4,1]]

def solution(description):
    if len(description) > 104 or len(description) < 1:
        print("Invalid Size")
        return
    node = {}
    for i in description:
        if len(i) != 3:
            print("Single description is nt 3! Error")
            return
        node[i[0]] = [i[1],i[2]]

  

# find root
    root = None
    for key in node:
        isRoot = True
        for value in node.values():
            if key == value[0]:
                isRoot = False
                break
        if isRoot:
            root = key    

# display
    Remaininglist = []
    output = []
    Remaininglist.append(root)
    output.append(root)
    i = 0    
    while i < len(description):
        current = Remaininglist.pop(0)
        print(current)
        curLen = 0
        for j in description:
            if current == j[0]:
                curLen += 1
        print(curLen)        
                
        if curLen == 1:   
            print(node[current])         
            if node[current][1] == 1:
                output.append(node[current][0])
                output.append(None)

                Remaininglist.append(node[current][0])
            else:
                output.append(None)
                output.append(node[current][0])
                Remaininglist.append(node[current][0])
        elif curLen == 0:
            # do nothing
            print("", end="")
            output.append(None)
            output.append(None)
        else:
            i = i+1
            left = 0
            right = 0
            for j in description:
                if j[0] == current and j[2] == 1:
                    left = j[1]
                if j[0] == current and j[2] == 0:
                    right = j[1]
            Remaininglist.append(left)
            Remaininglist.append(right)
            output.append(left)
            output.append(right)    
        i+=1    

    output = ["null" if x is None else x for x in output] 
    while True:    
        last = output.pop()
        if last != "null":
            output.append(last)
            break
    print(output)   


solution(description)


        








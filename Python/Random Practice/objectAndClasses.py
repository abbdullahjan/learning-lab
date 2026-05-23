x = 10 
y = 10
print(f'x={id(x)} | y={id(y)}')
x=20
print(y)
print(f'x={id(x)} | y={id(y)}')


# ------------------------------------------------------
# class Rectangle:
#     def __init__(self, width=1, height=2):
#         self.width = width
#         self.height = height

#     def getArea(self):
#         return self.width * self.height

#     def getPerimeter(self):
#         return self.width*2 + self.height*2
        
# def main():
#     r1 = Rectangle()
#     print(f'Area = {r1.getArea()}, Perimeter = {r1.getPerimeter()}')
#     r2 = Rectangle(5,10)
#     print(f'Area = {r2.getArea()}, Perimeter = {r2.getPerimeter()}')

# main()
# ------------------------------------------------------

# class Count:
#     def __init__(self, count = 0):
#         self.count = count

# def main():
#     c = Count()
#     times = 0
#     for i in range(100):
#         increment(c, times)
#     print("count is", c.count)
#     print("times is", times)
# def increment(c, times):
#     c.count += 1
#     times += 1
# main()

# ---------------------------------------------------

# class A:
#     radius = 1
#     def __init__(self, i = 5):
#         self.i = i

# def main():
#     a = A()
#     print(a.i)
#     incRadius(a)
#     print(a.radius)

# def incRadius(a):
#     a = A(10 );
#     a.radius = 200

# main()

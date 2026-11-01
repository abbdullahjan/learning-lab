import numpy as np
X = np.array([[1,2,3],[2,1,3]])
# X = np.array([[1,2],[2,1],[3,3]])
# X = X.T
Y = np.array([4,5,7])
w1 = 1
w2 = 2
b = 1
W = np.array([w1,w2])
alpha = 0.01

# Y = Y.reshape(1,-1)
print(X.shape)
print(Y.shape)
print(W.shape)
WT = W.reshape(1,-1)
print(W.shape)



# Prediction
Yhat = WT.dot(X) + b
# Yhat = Yhat[0]
print(Yhat)
print(Y)
E = Yhat - Y
print(E)

# cost = ((np.dot(E,E.T))/2*X.shape[1]).squeeze()
cost = ((np.dot(E,E.T))/(2*(X.shape[1]))).squeeze()
print(cost)

# W = W.reshape(2,1)
print("W= ",W.shape)
W = W - (alpha * (1/X.shape[1]) * ( np.dot(X,E.T)) ).squeeze()
b = b - alpha * (1/X.shape[1]) * (np.sum(E))


print((alpha*(1/X.shape[1]) * ( np.dot(X,E.T))))
print(W.shape)
print(b)

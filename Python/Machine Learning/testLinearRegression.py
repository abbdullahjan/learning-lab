# Example dataset for linear regression
# 3 houses, each with 3 features: size (sqft), bedrooms, age (years)
# Target: price (USD)

# Table format
# | House | Size (sqft) | Bedrooms | Age (years) | Price ($) |
# |------|-------------|----------|-------------|----------|
# | 1    | 2100        | 3        | 10          | 320000   |
# | 2    | 2400        | 4        | 5           | 380000   |
# | 3    | 1800        | 2        | 15          | 290000   |

import numpy as np
from sklearn.model_selection import train_test_split
from sklearn.datasets import load_diabetes
import pandas as pd

diabetes = load_diabetes()


df = pd.DataFrame(data=diabetes.data, columns=diabetes.feature_names)
print(df)

x = [
    [2100, 3, 10],
    [2400, 4, 5],
    [1800, 2, 15],
]

y = [320000, 380000, 290000]

xrows = 0
X = np.array(x).T
Y = np.array(y)
for i in X:
    xrows+=1
   
w = np.zeros(xrows)
print(X.shape[0])

b = 0.0

def calculateYHat(w,X,b):
    return np.dot(w,X) + b

def calculateError(yHat, y):
    return yHat - y

print(calculateError(calculateYHat(w,X,b), Y))

def calculateWdescent(E, w, b, X, alpha=0.0000001):
    m = X.shape[1] 
    
    gradient_w = (1 / m) * np.dot(X, E)
    newW = w - alpha * gradient_w
    
    gradient_b = (1 / m) * np.sum(E)
    newB = b - alpha * gradient_b
    
    return newW, newB

def UpdateValues(w, b, X, Y, iterations=10, alpha=0.0000001):
    for i in range(iterations):
        yHat = calculateYHat(w, X, b)
        E = calculateError(yHat, Y)
        w, b = calculateWdescent(E, w, b, X, alpha)
        print(f"Iteration {i+1} | w: {w} | b: {b:.4f}")
    return w, b

final_w, final_b = UpdateValues(w, b, X, Y)

x = [1, 2, 3, 4, 5]
y = [200, 300, 400, 500, 600]

w = 0
b = 0

alpha = 0.1
iterations = 10000


for _ in range(iterations):

    print(f"-----Iteration {_}-----")
    # 1. Make predictions
    predictions = []

    for i in range(len(x)):
        prediction = w * x[i] + b
        predictions.append(prediction)

    # 2. Calculate errors
    errors = []

    for i in range(len(x)):
        error = predictions[i] - y[i]
        errors.append(error)

    # 3. Calculate gradient for w
    dw = 0

    for i in range(len(x)):
        dw += (errors[i]) * x[i]

    dw = dw / len(x)

    # 4. Calculate gradient for b
    db = 0

    for i in range(len(x)):
        db += errors[i]

    db = db / len(x)

    # 5. Update parameters
    w = w - alpha * dw
    b = b - alpha * db

    print(f"Prediction: {predictions}")
    print(f"errors: {errors}")
    print(f"dw: {dw}")
    print(f"db: {db}")
    print("w =", w)
    print("b =", b)
    print("______iteration end______")


print("w =", w)
print("b =", b)


print("Now checking a value for a given input")
value = int(input("Ënter some value: "))
print(f"input:{value} | output: {w*value + b}")
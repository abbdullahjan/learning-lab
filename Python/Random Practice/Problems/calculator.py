import ast
import sys

ALLOWED_BIN_OPS = {
    ast.Add: lambda a, b: a + b,
    ast.Sub: lambda a, b: a - b,
    ast.Mult: lambda a, b: a * b,
    ast.Div: lambda a, b: a / b,
    ast.FloorDiv: lambda a, b: a // b,
    ast.Mod: lambda a, b: a % b,
    ast.Pow: lambda a, b: a ** b,
}

ALLOWED_UNARY_OPS = {
    ast.UAdd: lambda a: +a,
    ast.USub: lambda a: -a,
}


def _safe_eval(expression):
    try:
        parsed = ast.parse(expression, mode="eval")
    except SyntaxError as exc:
        raise ValueError(f"Invalid expression: {exc.msg}") from exc

    def evaluate(node):
        if isinstance(node, ast.Expression):
            return evaluate(node.body)
        if isinstance(node, ast.BinOp):
            left = evaluate(node.left)
            right = evaluate(node.right)
            op_type = type(node.op)
            if op_type not in ALLOWED_BIN_OPS:
                raise ValueError(f"Unsupported operator: {type(node.op).__name__}")
            return ALLOWED_BIN_OPS[op_type](left, right)
        if isinstance(node, ast.UnaryOp):
            operand = evaluate(node.operand)
            op_type = type(node.op)
            if op_type not in ALLOWED_UNARY_OPS:
                raise ValueError(f"Unsupported operator: {type(node.op).__name__}")
            return ALLOWED_UNARY_OPS[op_type](operand)
        if isinstance(node, ast.Constant) and isinstance(node.value, (int, float)):
            return node.value
        if isinstance(node, ast.Num):
            return node.n
        raise ValueError(f"Unsupported expression element: {type(node).__name__}")

    return evaluate(parsed)


def run_interactive():
    print("Simple command-line calculator")
    print("Type 'exit' or 'quit' to stop.")

    while True:
        try:
            user_input = input("calc> ")
        except EOFError:
            print()
            break

        command = user_input.strip()
        if not command:
            continue
        if command.lower() in {"exit", "quit", "q"}:
            break

        try:
            result = _safe_eval(command)
        except Exception as exc:
            print(f"Error: {exc}")
            continue

        print(result)


def main():
    if len(sys.argv) > 1:
        expression = " ".join(sys.argv[1:])
        try:
            result = _safe_eval(expression)
            print(result)
        except Exception as exc:
            print(f"Error: {exc}")
            sys.exit(1)
        return

    run_interactive()


if __name__ == "__main__":
    main()

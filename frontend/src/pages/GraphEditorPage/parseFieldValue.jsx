export function parseFieldValue(value, type, fieldName) {
    if (type === "int") {
        if (value === "") {
            return 0;
        }

        const number = Number(value);

        if (!Number.isInteger(number)) {
            throw new Error(`${fieldName} must be an integer`);
        }

        return number;
    }

    if (type === "float") {
        if (value === "") {
            return 0;
        }

        const number = Number(value);

        if (!Number.isFinite(number)) {
            throw new Error(`${fieldName} must be a number`);
        }

        return number;
    }

    if (type === "bool") {
        return Boolean(value);
    }

    return value ?? "";
}
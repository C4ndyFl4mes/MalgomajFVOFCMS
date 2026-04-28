export async function signin(request) {
    try {
        const response = await fetch("/api/user/signin", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            body: JSON.stringify(request)
        });

        const contentType = response.headers.get("content-type") || "";
        const payload = contentType.includes("application/json") ? await response.json() : null;

        if (!response.ok) {
            return {
                success: false,
                error: payload ?? {
                    statusCode: response.status,
                    type: "HttpError",
                    detail: "Inloggningen misslyckades."
                }
            };
        }

        return {
            success: true,
            data: payload
        };
    } catch (error) {
        return {
            success: false,
            error: {
                statusCode: 0,
                type: "NetworkError",
                detail: error?.message ?? "Nätverksfel."
            }
        };
    }
}
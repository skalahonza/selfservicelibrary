import api from "./api";

export async function signIn(code) {
    const response = await api.post("/api/Auth/sign-in", {
        code: code
    });
    const { access_token, refresh_token, expiresIn } = response.data;
    localStorage.setItem("access_token", access_token);
    localStorage.setItem("refresh_token", refresh_token);
    localStorage.setItem("expires_in", expiresIn);
}

export async function refresh() {
    const response = await api.post("/api/Auth/refresh", {
        refreshToken: localStorage.getItem("refresh_token")
    });
    const { access_token, refresh_token, expiresIn } = response.data;
    localStorage.setItem("access_token", access_token);
    localStorage.setItem("refresh_token", refresh_token);
    localStorage.setItem("expires_in", expiresIn);
}

export function isExpired(){
    const expiresIn = localStorage.getItem("expires_in");
    const now = new Date();
    return now >= Date.parse(expiresIn);
}
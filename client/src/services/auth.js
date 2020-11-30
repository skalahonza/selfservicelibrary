import api from "./api";

export async function signIn(code) {
    const response = await api.post("/api/Auth/sign-in", {
        code: code
    });
    const { access_token, refresh_token, expires_in } = response.data;
    localStorage.setItem("access_token", access_token);
    localStorage.setItem("refresh_token", refresh_token);
    localStorage.setItem("expires_in", expires_in);
    const {preferredEmail, fullName} = await userProfile();
    localStorage.setItem("preferredEmail", preferredEmail);
    localStorage.setItem("fullName", fullName);
}

export async function refresh() {    
    try {
        const response = await api.post("/api/Auth/refresh", {
            refreshToken: localStorage.getItem("refresh_token")
        });
        const { access_token, refresh_token, expires_in } = response.data;
        localStorage.setItem("access_token", access_token);
        localStorage.setItem("refresh_token", refresh_token);
        localStorage.setItem("expires_in", expires_in);
        return true;
    } catch (error) {
        return false;
    }
}

export async function userProfile(){
    const response = await api.get("/api/Users/me");
    return response.data;
}

export function isAuthorized(){
    const access_token = localStorage.getItem("access_token");
    const refresh_token = localStorage.getItem("refresh_token");
    if(access_token && refresh_token){
        return !isExpired();
    }
    return false;
}

export function isExpired(){
    const expiresIn = localStorage.getItem("expires_in");
    if(expiresIn == undefined) return true;
    const now = new Date();
    return now >= Date.parse(expiresIn);
}
import api from "./api";
import axios from "axios";

function store(access_token, refresh_token, expires_in) {
    const now = new Date();    
    now.setSeconds(now.getSeconds() + expires_in - 60);
    localStorage.setItem("access_token", access_token);
    localStorage.setItem("refresh_token", refresh_token);
    localStorage.setItem("expires_in", now);
}

export async function signIn(code) {
    signOut();
    // creating another instance to prevent axios interceptor infinite loops
    const instance = axios.create({
        baseURL: process.env.VUE_APP_API_URL,
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json"
        }
    });
    const response = await instance.post("/api/Auth/sign-in", {
        code: code
    });
    const {
        access_token,
        refresh_token,
        expires_in
    } = response.data;
    store(access_token, refresh_token, expires_in);
    const {
        preferredEmail,
        fullName
    } = await userProfile();
    localStorage.setItem("preferredEmail", preferredEmail);
    localStorage.setItem("fullName", fullName);
}

export function signOut(){
    localStorage.removeItem("access_token");
    localStorage.removeItem("refresh_token");
    localStorage.removeItem("expires_in");
    localStorage.removeItem("preferredEmail");
    localStorage.removeItem("fullName");
}

export async function refresh() {
    try {
        // creating another instance to prevent axios interceptor infinite loops
        const instance = axios.create({
            baseURL: process.env.VUE_APP_API_URL,
            headers: {
                "Accept": "application/json",
                "Content-Type": "application/json"
            }
        });

        const response = await instance.post("/api/Auth/refresh", {
            refreshToken: localStorage.getItem("refresh_token")
        });
        const {
            access_token,
            refresh_token,
            expires_in
        } = response.data;
        store(access_token, refresh_token, expires_in);
        return true;
    } catch (error) {
        return false;
    }
}

export async function userProfile() {
    const response = await api.get("/api/Users/me");
    return response.data;
}

export function isAuthorized() {
    const access_token = localStorage.getItem("access_token");
    const refresh_token = localStorage.getItem("refresh_token");
    if (access_token && refresh_token) {
        return !isExpired();
    }
    return false;
}

export function isExpired() {
    const expiresIn = localStorage.getItem("expires_in");
    if (!expiresIn) 
        return true;
    const now = new Date();
    return now >= Date.parse(expiresIn);
}
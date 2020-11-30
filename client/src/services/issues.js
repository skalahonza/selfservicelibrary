import api from "./api";

export async function getMine(){
    const response = await api.get("/api/Users/me/issues");
    return response.data;
}
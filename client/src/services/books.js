import api from "./api";

export async function getAll(){
    const response = await api.get("/api/Books");
    return response.data;
}
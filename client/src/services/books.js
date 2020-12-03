import api from "./api";

export async function getAll(){
    const response = await api.get("/api/Books");
    return response.data;
}

export async function getDetail(id){
    const response = await api.get(`/api/Books/${id}`);
    return response.data;
}

export function addBook(book){
    return api.post("/api/Books", book);
}

export async function editBook(id, book){
    const response = await api.patch(`/api/Books/${id}`, book);
    return response.data;
}

export function borrowBook(bookId){
    return api.post(`/api/Issues/${bookId}/borrow`);
}
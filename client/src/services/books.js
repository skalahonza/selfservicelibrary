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

export function editBook(book){
    return api.patch(`/api/Books/${book.id}`, book);
}

export function borrowBook(bookId){
    return api.post(`/api/Issues/${bookId}/borrow`);
}
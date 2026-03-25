import axios from "axios";

const API = "https://localhost:7157/api/auth";

export const registerUser = (data) => {
  return axios.post(`${API}/register`, data);
};
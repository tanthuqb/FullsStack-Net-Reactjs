import { useState } from "react";
import { useNavigate } from "react-router-dom";


export default function Login() {
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const navigate = useNavigate();

    const handleLogin = async (e) => {
        e.preventDefault();

        const res = await fetch("http://localhost:5000/api/Auth/login", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            credentials: "include",
            body: JSON.stringify({ username, password }),
        });

        if (res.ok) {
            const data = await res.json();
            navigate("/tasks", {
                state: { username: data.username, token: data.token },
            });
        } else {
            const toastEl = document.getElementById("loginToast");
            const toast = new window.bootstrap.Toast(toastEl);
            toast.show();
        }
    };

    return (
        <div className="d-flex vh-100 justify-content-center align-items-center bg-light">
            <form
                onSubmit={handleLogin}
                className="bg-white p-4 w-100 rounded shadow"
                style={{ maxWidth: 400 }}
            >
                <h2 className="mb-4 text-center">Login</h2>
                <div className="mb-3">
                    <input
                        type="text"
                        className="form-control"
                        placeholder="Username"
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                        required
                    />
                </div>
                <div className="mb-3">
                    <input
                        type="password"
                        className="form-control"
                        placeholder="Password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    />
                </div>
                <button type="submit" className="btn btn-primary w-100">
                    Login
                </button>
            </form>

            {/* Toast */}
            <div
                className="toast align-items-center text-bg-danger position-fixed bottom-0 end-0 m-3 border-0"
                role="alert"
                aria-live="assertive"
                aria-atomic="true"
                id="loginToast"
            >
                <div className="d-flex">
                    <div className="toast-body">
                        Đăng nhập thất bại. Vui lòng kiểm tra lại tài khoản.
                    </div>
                    <button
                        type="button"
                        className="btn-close btn-close-white me-2 m-auto"
                        data-bs-dismiss="toast"
                        aria-label="Close"
                    ></button>
                </div>
            </div>
        </div>
    );
}

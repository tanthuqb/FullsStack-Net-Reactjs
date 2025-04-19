import { useLocation, useNavigate } from "react-router-dom";
import { useState, useEffect } from "react";
import AddAndEditTaskForm from "./components/AddAndEditTaskForm";
import TaskTable from "./components/TaskTable";


export default function TaskManager() {
    const { state } = useLocation();
    const username = state?.username || "Guest";
    const navigate = useNavigate();
    const [editingTask, setEditingTask] = useState(null);

    useEffect(() => {
        fetch("http://localhost:5000/api/Auth/me", {
            credentials: "include"
        }).then(res => {
            if (res.status === 401) navigate("/");
        }).catch(() => navigate("/"));
    }, [navigate]);

    const handleLogOut = async () => {
        await fetch("http://localhost:5000/api/Auth/logout", {
            method: "POST",
            credentials: "include"
        });
        navigate("/");
    };

    const handleEdit = (task) => setEditingTask(task);
    const handleDelete = async (id) => {
        await fetch(`http://localhost:5000/api/TaskEntity/${id}`, {
            method: "DELETE",
            credentials: "include"
        });
        window.location.reload();
    };

    return (
        <div className="py-5 container">
            <div className="d-flex justify-content-between align-items-center mb-4">
                <h2>Hello, {username}</h2>
                <button className="btn btn-danger" onClick={handleLogOut}>Logout</button>
            </div>
            <AddAndEditTaskForm editingTask={editingTask} onAdd={() => window.location.reload()} onUpdate={() => window.location.reload()} clearEdit={() => setEditingTask(null)} />
            <TaskTable onEdit={handleEdit} onDelete={handleDelete} />
        </div>
    );
}

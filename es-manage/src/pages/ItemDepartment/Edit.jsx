import React, { useState, useEffect } from "react";
import axios from "axios";
import { useParams, useNavigate, Link } from "react-router-dom";
import AppLayout from "../../layouts/AppLayout";

function EditItemDepartment() {
  const { id: urlId, categoryName: urlCategoryName } = useParams();
  const [id, setId] = useState(urlId);
  const [categoryName, setCategoryName] = useState(urlCategoryName);
  const [itemDepartmentParentId, setItemDepartmentParentId] = useState("");
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [parent, setParent] = useState(true);

  const navigate = useNavigate();

  const getToken = () => {
    return localStorage.getItem("token");
  };

  const fetchData = async () => {
    setIsLoading(true);
    try {
      const response = await axios.get(
        `https://localhost:7240/api/itemdepartment/${id}/${categoryName}`,
        {
          headers: {
            Authorization: `Bearer ${getToken()}`, // Include the token from local storage
          },
        }
      );
      setId(response.data.id);
      setCategoryName(response.data.categoryName);
      setItemDepartmentParentId(response.data.itemDepartmentParentId);
    } catch (error) {
      console.error(error);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, [id, categoryName]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    const effectiveItemDepartmentParentId = parent
      ? "0"
      : itemDepartmentParentId;

    try {
      const response = await axios.put(
        `https://localhost:7240/api/itemdepartment/${id}/${categoryName}`,
        {
          id: id,
          categoryName: categoryName,
          itemDepartmentParentId: effectiveItemDepartmentParentId,
        },
        {
          headers: {
            Authorization: `Bearer ${getToken()}`, // Include the token from local storage
          },
        }
      );
      console.log(response.data);
      navigate("/item-department"); // Update this with the correct path to your Dashboard component
    } catch (error) {
      console.error(error);
      setError(JSON.stringify(error, Object.getOwnPropertyNames(error)));
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <AppLayout>
      <h2 className="page-title">Create Item Department</h2>
      <div className="card mt-3">
        <div className="card-body">
          <form onSubmit={handleSubmit}>
            <div className="mb-3">
              <label className="form-label">ID</label>
              <input
                type="text"
                className="form-control"
                placeholder="Enter the ID"
                value={id}
                onChange={(e) => setId(e.target.value)}
                disabled
              />
            </div>
            <div className="mb-3">
              <label className="form-label">Category Name</label>
              <input
                type="text"
                className="form-control"
                placeholder="Enter the category name"
                value={categoryName}
                onChange={(e) => setCategoryName(e.target.value)}
                disabled
              />
            </div>
            <div className="mb-3">
              <label className="form-check form-switch">
                <input
                  className="form-check-input"
                  type="checkbox"
                  checked={parent}
                  onChange={() => setParent(!parent)}
                />
                <span className="form-check-label">Use as a parent?</span>
              </label>
            </div>
            <div className="mb-3">
              <label className="form-label">Item Department Parent ID</label>
              <input
                type="text"
                className="form-control"
                placeholder="Enter the item department parent ID"
                value={itemDepartmentParentId}
                onChange={(e) => setItemDepartmentParentId(e.target.value)}
                disabled={parent}
              />
            </div>
            <div className="mb-3">
              <input type="submit" value="Save" className="btn btn-primary" />
            </div>
            <div className="mb-3">
              <Link to="/item-department" className="btn btn-primary">
                Cancel
              </Link>
            </div>
          </form>
          {error && (
            <div className="alert alert-danger" role="alert">
              {error}
            </div>
          )}
        </div>
      </div>
    </AppLayout>
  );
}

export default EditItemDepartment;

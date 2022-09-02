import { Col, Row, Input, Button, Select } from 'antd';
import { React, useEffect, useState } from 'react'
import Cookies from "js-cookie";
import { SaveOutlined } from '@ant-design/icons';
import TableData from "../Shared/TableData";
import axios from "axios";
import { AddUser } from './AddUser';
import { useHistory } from "react-router";
import Config from "../../helpers/Config";


const UserManagement = ({ user }) => {

	const API_URL = "https://localhost:7252/api"

	const [isModalAddUserVisible, setModalAddUserVisible] = useState(false);
	const [data, setData] = useState([]);
	const [isLoaded, setIsLoaded] = useState(false);
	const history = useHistory();
	const authorization = Cookies.get("access-token");


	const showAddUserModal = () => {
		setModalAddUserVisible(true);
	}

	const columns = [
		{
			title: "Nombre",
			dataIndex: "name",
			key: "name",
		},
		{
			title: "Correo",
			dataIndex: "email",
			key: "email",
		},
		{
			title: "Rol",
			dataIndex: "role",
			key: "role",
			render: (text, record) => (
				<span>
					{record.role === 1 && 'Agente'}
					{record.role === 0 && 'Administrador'}
				</span>
			)
		},
	];

	const handleChangeStatus = (user) => {

		axios
			//.post(`${API_URL}/User/disable-user?id=${user.userId}`,
			.post(`${Config.API_URL}/User/disable-user?id=${user.userId}`,
				{
					headers: { Authorization: "Bearer " + authorization },
				}
			)
			.then(res => {
				if (res.data.success)
					setIsLoaded(false)
			})
			.catch(err => {
			});
	}


	useEffect(() => {

		if (!isLoaded) {
			setIsLoaded(true);

			axios.get(`${Config.API_URL}/User/get-all`,
				//axios.get(`${API_URL}/User/get-all`,
				{
					headers: { Authorization: "Bearer " + authorization },
				}
			)
				.then((res) => {
					if (res.data) {
						if (res.data.status === 401) {
							history.push("/login")
							return null
						} else {

							setData(res.data)

						}
					}
				})
				.catch(() => {
					history.push("/login");
				})
				.finally(() => {
					setIsLoaded(true);
				});
		}
	}, [isLoaded])

	return (
		<>
			<div style={{ textAlign: 'center' }}>
				<h3>Usuarios</h3>
			</div>
			<br />
			<Row gutter={16}>
				<Col span={4}>
					<Button onClick={showAddUserModal} type="primary" className='btn-primary' size="large" icon={<SaveOutlined />}>
						Registrar
					</Button>
				</Col>
			</Row>
			<br />
			<Row>
				<Col span={24}>
					<TableData
						data={data}
						columns={columns}
						loading={!isLoaded}
						userStatusAction={handleChangeStatus}
						showActions={true}
					/>
				</Col>
			</Row>
			<AddUser
				loading={!isLoaded}
				setLoading={setIsLoaded}
				isModalAddUserVisible={isModalAddUserVisible}
				setModalAddUserVisible={setModalAddUserVisible}
			/>
		</>
	)
}
export default UserManagement;
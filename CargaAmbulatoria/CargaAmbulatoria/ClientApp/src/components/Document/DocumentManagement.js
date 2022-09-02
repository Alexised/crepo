import { Col, Row, Input, Button, Select } from 'antd';
import { React, useEffect, useState } from 'react'
import Cookies from "js-cookie";
import { SaveOutlined } from '@ant-design/icons';
import TableData from "../Shared/TableData";
import axios from "axios";
import { AddDocument } from './AddDocument';
import { useHistory } from "react-router";
import { format } from 'date-fns'
import Config from "../../helpers/Config";


const DocumentManagement = ({ user }) => {

	const API_URL = "https://localhost:7252/api"

	const [isModalAddDocVisible, setModalAddDocVisible] = useState(false);
	const [data, setData] = useState([]);
	const [isLoaded, setIsLoaded] = useState(false);
	const history = useHistory();
	const authorization = Cookies.get("access-token");


	const showAddUserModal = () => {
		setModalAddDocVisible(true);
	}

	const columns = [
		{
			title: "Fecha",
			dataIndex: "documentDate",
			key: "documentDate",
			render: (text, record) => (
				<span>
					{format(new Date(record.documentDate), 'dd/MM/yyyy')}
				</span>
			)
		},
		{
			title: "Nombre",
			dataIndex: "name",
			key: "name",
		},
		{
			title: "Ruta",
			dataIndex: "path",
			key: "path",
		}
	];

	const handleChangeStatus = (doc) => {
		//setIsLoaded(true)
		axios
			//.post(`${API_URL}/Document/delete-document?id=${doc.documentId}`,
			.post(`${Config.API_URL}/Document/delete-document?id=${doc.documentId}`, null,
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
			//setIsLoaded(true)

			axios.get(`${Config.API_URL}/Document/get-documentByUserId?id=${user.userId}`,
			//axios.get(`${API_URL}/Document/get-documentByUserId?id=${user.userId}`,
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
	}, [isLoaded, authorization, user])

	return (
		<>
			<div style={{ textAlign: 'center' }}>
				<h3>Documentos</h3>
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
						docDeleteAction={handleChangeStatus}
						showActions={true}
					/>
				</Col>
			</Row>
			<AddDocument
				loading={!isLoaded}
				userId={user.userId}
				setLoading={setIsLoaded}
				isModalAddDocVisible={isModalAddDocVisible}
				setModalAddDocVisible={setModalAddDocVisible}
			/>
		</>
	)
}
export default DocumentManagement;
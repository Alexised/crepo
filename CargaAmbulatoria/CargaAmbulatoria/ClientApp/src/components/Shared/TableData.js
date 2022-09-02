import { Button, Col, Row, Table, Tooltip } from "antd";
import { Content } from "antd/lib/layout/layout";
import { React } from 'react'
import { LikeOutlined, DislikeOutlined, DeleteOutlined, EditOutlined } from '@ant-design/icons';


const TableData = ({
    data,
    columns,
    userStatusAction,
    docDeleteAction,
    showActions
}) => {
    const tableColumns = [...columns]

    if (showActions) {

        tableColumns.push({
            title: 'Acciones',
            key: 'acciones',
            dataIndex: 'acciones',

            render: (text, record) => (
                <div style={{ display: "inline-flex" }}>

                    {
                        (userStatusAction && record.status !== 0) &&
                        <Tooltip title="Activar Usuario">
                            <Button
                                type="primary"
                                onClick={() => userStatusAction(record)}
                                style={{ background: "#0650A0", borderColor: "#0650A0", marginRight: "5px" }}
                                icon={<LikeOutlined />}
                            />
                        </Tooltip>
                    }
                    {
                        userStatusAction && record.status === 0 &&
                        <Tooltip title="Desactivar Usuario">
                            <Button
                                type="primary"
                                onClick={() => userStatusAction(record)}
                                style={{ background: "#a00606", borderColor: "#a00606", marginRight: "5px" }}
                                icon={<DislikeOutlined />}
                            />
                        </Tooltip>
                    }
                    
                    {
                        docDeleteAction &&
                        <Tooltip title="Eliminar documento">
                            <Button
                                type="primary"
                                onClick={() => docDeleteAction(record)}
                                style={{ background: "#a00606", borderColor: "#a00606", marginRight: "5px" }}
                                icon={<DeleteOutlined />}
                            />
                        </Tooltip>
                    }

                </div>
            ),
        })
    }

    return (
        <>
            <Content style={{ background: "#ffffff" }}>
                <Row style={{ marginTop: "10px" }} align="center">
                    <Col span={24} align="center">
                        <Table scroll={{ x: 400 }} loading={false} dataSource={data} columns={tableColumns} bordered />
                    </Col>
                </Row>
            </Content>
        </>
    )
}

export default TableData;
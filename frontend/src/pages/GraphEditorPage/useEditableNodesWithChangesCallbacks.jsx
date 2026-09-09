import { useCallback, useMemo } from 'react';
import { parseFieldValue } from "./parseFieldValue";


export function useEditableNodesWithChangesCallbacks({ nodes, setNodes, schemas }) {
	const handleNodeFieldChange = useCallback((nodeId, fieldName, value, type) => {
		setNodes((nodes) =>
			nodes.map((node) => {
				if (node.id !== nodeId) {
					return node;
				}

				return {
					...node,
					data: {
						...node.data,
						properties: {
							...node.data.properties,
							[fieldName]: parseFieldValue(value, type, fieldName),
						},
					},
				};
			})
		);
	}, [setNodes]);

	const handleNodeTitleChange = useCallback((nodeId, title) => {
		setNodes((nodes) =>
			nodes.map((node) => {
				if (node.id !== nodeId) {
					return node;
				}

				return {
					...node,
					data: {
						...node.data,
						title,
					},
				};
			})
		);
	}, [setNodes]);

	const handleNodeSchemaTypeChange = useCallback((nodeId, schemaTypeName) => {
		setNodes((nodes) =>
			nodes.map((node) => {
				if (node.id !== nodeId) {
					return node;
				}

				return {
					...node,
					data: {
						...node.data,
                        schemaTypeName,
                        properties: {}
					},
				};
			})
		);
	}, [setNodes]);

	const nodesWithCallbacks = useMemo(() => nodes.map((node) => ({
		...node,
		data: {
			...node.data,
			onFieldChange: handleNodeFieldChange,
			onTitleChange: handleNodeTitleChange,
			onSchemaTypeChange: handleNodeSchemaTypeChange,
			schemas
		},
	})), [nodes, handleNodeFieldChange, handleNodeTitleChange, handleNodeSchemaTypeChange, schemas]);

	return nodesWithCallbacks;
}

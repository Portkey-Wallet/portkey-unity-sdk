import type {CodegenConfig} from '@graphql-codegen/cli';

const config: CodegenConfig = {
    overwrite: true,
    schema: "did_schema.graphql",
    generates: {
        "../Assets/Portkey/Scripts/__Generated__/GraphQLCodeGen.cs": {
            plugins: ["c-sharp"]
        }
    }
};

export default config;
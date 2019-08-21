<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:md="http://genx.com/metadata" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="text" encoding="UTF-8" indent="yes" />
<xsl:preserve-space elements="*"/>

<!-- Mandatory parameter from the genx cli -->
<xsl:param name="EntityName" />

<!-- Custom parameters -->
<xsl:param name="OrganisationName" />
<xsl:param name="ApplicationName" />
<xsl:param name="ServiceName" />

<xsl:template match="/">
   <xsl:apply-templates select="//md:entities/md:entity[@name=$EntityName]/md:entitycolumns" />
</xsl:template>

<xsl:template match="md:entitycolumns">
<xsl:variable name="EntityName" select="ancestor::md:entity/@name"/>
<xsl:variable name="EntityOriginalName" select="ancestor::md:entity/@originalname"/>
using <xsl:value-of select="$OrganisationName"/>.Dotnet.Core.Command;
using MediatR;
using System;
using System.Runtime.Serialization;

// TODO: Update Namespace.
namespace <xsl:value-of select="$ApplicationName"/>.<xsl:value-of select="$ServiceName"/>.Core.Commands
{
    [DataContract]
    public class Create<xsl:value-of select="$EntityName"/>Command : CommandRequest, IRequest&lt;CommandResponse&lt;bool&gt;&gt;
    {
<xsl:for-each select="md:column"><xsl:if test="@isprimarykey='false'">
        [DataMember]
		public <xsl:call-template name="sqltocsharp"><xsl:with-param name="entitycolumn" select="."/></xsl:call-template><xsl:text> </xsl:text><xsl:value-of select="@name"/> { get; private set; }
</xsl:if></xsl:for-each>
<xsl:apply-templates select="//md:entities/md:entity[@name=$EntityName]/md:relationships" mode="properties" />
		public Create<xsl:value-of select="$EntityName"/>Command()
		{
			<xsl:apply-templates select="//md:entities/md:entity[@name=$EntityName]/md:relationships" mode="default_constructor" />
		}

		public Create<xsl:value-of select="$EntityName"/>Command(
			<xsl:apply-templates select="//md:entities/md:entity[@name=$EntityName]/md:relationships" mode="constructor" />
            <xsl:for-each select="md:column"><xsl:if test="@isprimarykey='false'">
            <xsl:call-template name="sqltocsharp"><xsl:with-param name="entitycolumn" select="."/></xsl:call-template><xsl:text> </xsl:text><xsl:value-of select="@camelcase"/><xsl:if test="position()!=last()"><xsl:text>,
            </xsl:text></xsl:if>
            </xsl:if></xsl:for-each>
		)
		{
			<xsl:apply-templates select="//md:entities/md:entity[@name=$EntityName]/md:relationships" mode="assignment" />
            <xsl:for-each select="md:column"><xsl:if test="@isprimarykey='false'">
            <xsl:value-of select="@name"/> = <xsl:value-of select="@camelcase"/>;
            </xsl:if></xsl:for-each>
		}
	}
}		
</xsl:template>

<xsl:template match="md:relationships" mode="properties">
<xsl:for-each select="md:relationship">
        private readonly List&lt;<xsl:value-of select="@foreignkeyentity"/>Dto&gt; _<xsl:value-of select="@foreignkeyentitycamelcase"/>s;
        public IEnumerable&lt;<xsl:value-of select="@foreignkeyentity"/>Dto&gt; <xsl:value-of select="@foreignkeyentity"/>s => _<xsl:value-of select="@foreignkeyentitycamelcase"/>s;
</xsl:for-each>
</xsl:template>

<xsl:template match="md:relationships" mode="constructor">
<xsl:for-each select="md:relationship">
            IEnumerable&lt;<xsl:value-of select="@foreignkeyentity"/>Dto&gt; <xsl:value-of select="@foreignkeyentitycamelcase"/>s,
			</xsl:for-each>
</xsl:template>

<xsl:template match="md:relationships" mode="assignment">
<xsl:for-each select="md:relationship">
            _<xsl:value-of select="@foreignkeyentitycamelcase"/>s = _<xsl:value-of select="@foreignkeyentitycamelcase"/>s.ToList();
			</xsl:for-each>
</xsl:template>

<xsl:template match="md:relationships" mode="default_constructor">
<xsl:for-each select="md:relationship">
            _<xsl:value-of select="@foreignkeyentitycamelcase"/>s = new List&lt;<xsl:value-of select="@foreignkeyentity"/>Dto&gt;();
			</xsl:for-each>
</xsl:template>

<xsl:template name="sqltocsharp">
	<xsl:param name="entitycolumn"/>
	<xsl:choose>
		<xsl:when test="@datatype='bigint'">long</xsl:when>
		<xsl:when test="@datatype='binary'">byte[]</xsl:when>
		<xsl:when test="@datatype='bit'">bool</xsl:when>
		<xsl:when test="@datatype='char'">string</xsl:when>
		<xsl:when test="@datatype='datetime'">DateTime</xsl:when>
		<xsl:when test="@datatype='decimal'">decimal</xsl:when>
		<xsl:when test="@datatype='float'">double</xsl:when>
		<xsl:when test="@datatype='int'">int</xsl:when>
		<xsl:when test="@datatype='image'">byte[]</xsl:when>
		<xsl:when test="@datatype='money'">decimal</xsl:when>
		<xsl:when test="@datatype='nchar'">string</xsl:when>
		<xsl:when test="@datatype='numeric'">decimal</xsl:when>
		<xsl:when test="@datatype='ntext'">string</xsl:when>
		<xsl:when test="@datatype='nvarchar'">string</xsl:when>
		<xsl:when test="@datatype='real'">Single</xsl:when>
		<xsl:when test="@datatype='smallint'">short</xsl:when>
		<xsl:when test="@datatype='text'">string</xsl:when>
		<xsl:when test="@datatype='tinyint'">byte</xsl:when>
		<xsl:when test="@datatype='uniqueidentifier'">Guid</xsl:when>		
		<xsl:when test="@datatype='varbinary'">byte[]</xsl:when>		
		<xsl:when test="@datatype='varchar'">string</xsl:when>		
	</xsl:choose>
</xsl:template>

</xsl:stylesheet> 
